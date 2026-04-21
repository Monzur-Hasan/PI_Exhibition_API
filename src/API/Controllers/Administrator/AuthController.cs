using API.Controllers.Base;
using Application.Features.Service.Email_Setting;
using Application.IdentityObject;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using Domain.Models.Access.DomainModel;
using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Login.Request;
using Domain.Models.Administrator.Login.Reset_Password;
using Domain.Models.Administrator.Login.Result;
using Domain.Shared.Configurations;
using Domain.Shared.Helpers;
using Domain.Shared.Settings;
using Domain.ViewModels.OTP;
using Infrastructure.Features.Repository.Interface;
using Infrastructure.Features.Service.Administrator;
using Infrastructure.Features.Service.Token_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace API.Controllers.Administrator
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private ForgotPasswordEmailValidator _passwordEmailValidator;
        private readonly IProjectRepository _projectRepository;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        public AuthController(ForgotPasswordEmailValidator passwordEmailValidator, IProjectRepository projectRepository, IConfiguration configuration, IJwtTokenService jwtTokenService)
        {
            _passwordEmailValidator = passwordEmailValidator;
            _projectRepository = projectRepository;
            _configuration = configuration;
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet("GetByPhone")]
        public async Task<IActionResult> GetByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("Phone number is required.");

            var user = await _projectRepository.GetUserByPhoneNumberAsync(phoneNumber);

            return CustomResult(
                "Request processed successfully.",
                new
                {
                    isExisting = user != null,
                    data = user
                },
                HttpStatusCode.OK
            );
        }

        [HttpPost, Route("Register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    });

                return BadRequest(errors);
            }

            // Validate reCAPTCHA token
            //var isCaptchaValid = await VerifyReCaptcha(request.Recaptcha);
            //if (!isCaptchaValid)
            //{
            //    return BadRequest("reCAPTCHA validation failed.");
            //}

            var imagePaths = new List<string>();

            var basePath = _configuration["FileUploadSettings:BasePath"];
            if (string.IsNullOrEmpty(basePath))
                basePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);           

            if (request.ProjectImages?.Count > 0)
            {
                foreach (var file in request.ProjectImages)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    var path = Path.Combine(basePath, fileName);

                    using var stream = new FileStream(path, FileMode.Create);
                    await file.CopyToAsync(stream);

                    imagePaths.Add(path);
                }
            }

            var result = await _projectRepository.UpsertUserProjectAsync(request, imagePaths);

            if (!result.Success)
            {
                return CustomResult(
                    result.Message,
                    null,
                    HttpStatusCode.BadRequest
                );
            }

            return CustomResult(
                result.Message,
                new { ProjectId = result.ProjectId},
                HttpStatusCode.OK
            );
        }

        // Verify reCAPTCHA with Google
        private static async Task<bool> VerifyReCaptcha(string token)
        {
            var secretKey = "6LcnJ1QrAAAAAFv4n_EIQ3XfnseImlQLqU0liWaD";
            using var client = new HttpClient();
            var response = await client.PostAsync(
                $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
                null
            );

            var json = await response.Content.ReadAsStringAsync();
            //Console.WriteLine($"Google reCAPTCHA Response: {json}");  // Log raw JSON response

            var result = JsonSerializer.Deserialize<RecaptchaResponse>(json);

            if (result == null)
            {
                //Console.WriteLine("Failed to deserialize reCAPTCHA response.");
                return false;
            }

            if (!result.Success)
            {
                //Console.WriteLine($"reCAPTCHA validation failed. Errors: {string.Join(", ", result.ErrorCodes ?? new List<string>())}");
            }

            return result.Success;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequestDto request)
        {
            var user = await _projectRepository.GetUserByPhoneNumberAsync(request.phoneNumber);
            if (user == null)
                return BadRequest("Invalid phone number");

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var deviceInfo = Request.Headers["User-Agent"].ToString();
            await _projectRepository.SaveRefreshTokenAsync(user.Id, refreshToken, deviceInfo);

            return Ok(new
            {
                Message = "Logged in successfully",
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var deviceInfo = Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Invalid request");

            var storedToken = await _projectRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            var user = await _projectRepository.GetuserByIdAsync(storedToken.UserId);
            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

            // Revoke old token
            await _projectRepository.RevokeRefreshTokenAsync(storedToken.Id);
            // Save new token
            await _projectRepository.SaveRefreshTokenAsync(user.Id, newRefreshToken, deviceInfo);

            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }


        [HttpPost("ValidateToken")]
        public IActionResult ValidateToken([FromBody] ValidateTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.Token))
                return BadRequest("Token required");

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = AppSettings.GetSymmetricSecurityKey();

                tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = AppSettings.JwtIssuer,
                    ValidAudience = AppSettings.JwtAudience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Ok("Token is valid");
            }
            catch (Exception ex)
            {
                return Unauthorized("Invalid or expired token");
            }
        }      


        [HttpPost, Route("LogoutAll")]
        public async Task<IActionResult> LogoutAllAsync([FromBody] string phoneNumber)
        {
            var user = await _projectRepository.GetUserByPhoneNumberAsync(phoneNumber);
            if (user == null)
                return NotFound("User not found");

            await _projectRepository.RevokeAllTokensAsync(user.Id);

            return Ok("Logged out from all devices");
        }

        [HttpPost, Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] OTPRequestsViewModel model)
        {
            try
            {
                bool hasEmail = await _passwordEmailValidator.FindByEmailAddressAsync(model.Email);
                if (hasEmail)
                {
                    // OTP Generate
                    int otp = new Random().Next(100000, 999999);
                    model.OTP = otp;

                    // Save OTP
                    var isSaveOTP = await _passwordEmailValidator.SaveOTPRequestsAsync(model);
                    if (isSaveOTP.Status)
                    {
                        string toEmail = model.Email;
                        string otpStr = otp.ToString();

                        // Get email settings
                        var emailSettings = await _passwordEmailValidator.GetEmailSettingAsync("OTP");
                        var data_list = await _passwordEmailValidator.GetOTPExpirationTimeAsync(model.Email);
                        if (data_list != null)
                        {
                            // Enqueue  email
                            //_emailQueueService.Enqueue(new QueuedEmail<string>
                            //{
                            //    ToEmail = toEmail,
                            //    EmailSetting = emailSettings,
                            //    BodyFormatter = otpValue => EmailTemplate.ForgotPasswordOTP(otpValue),
                            //    Data = otpStr,
                            //    Flag = "credentials"
                            //});

                            return CustomResult("OTP has been sent your email.",
                            new
                            {
                                isSaveOTP.Status,
                                otpLifeTime = data_list.OTPLifeTime,
                                currentTime = data_list.CreatedDate
                            }, HttpStatusCode.OK);
                        }

                    }
                }
                else
                {
                    return CustomResult("Email doesn't exist!", HttpStatusCode.NotFound);
                }

                return CustomResult("Something Went Wrong", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                // Optional: log the error
                return BadRequest(ResponseMessage.SomthingWentWrong);
            }
        }


        [HttpGet, Route("GetOTPExpirationTime")]
        public async Task<IActionResult> GetOTPExpirationTimeAsync([FromQuery] string email)
        {
            try
            {
                var data_list = await _passwordEmailValidator.GetOTPExpirationTimeAsync(email);
                if (data_list == null)
                {
                    return CustomResult("No data found.", HttpStatusCode.NotFound);
                }

                return CustomResult("Data loaded successfully.", new
                {
                    List = data_list,

                }, HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return CustomResult("Data loading failed!", HttpStatusCode.BadRequest);
            }
        }

        [HttpPost, Route("OTPVerification")]
        public async Task<IActionResult> OTPVerificationAsync([FromBody] OTPVerificationViewModel model)
        {
            try
            {
                var otpInfo = await _passwordEmailValidator.GetOTPInfoAsync(model.OTP, model.Email);
                if (otpInfo == null)
                {
                    return CustomResult("Invalid OTP!", HttpStatusCode.NotFound);
                }
                // Check expiration
                if (otpInfo.OTPLifeTime <= DateTime.Now)
                {
                    return CustomResult("OTP has expired!", HttpStatusCode.BadRequest);
                }

                // OTP is valid and not expired — update SendOTPTime
                var isUpdated = await _passwordEmailValidator.UpdateSendOTPTimeAsync(model.Email, model.OTP);
                if (isUpdated)
                {
                    var hasData = await _passwordEmailValidator.FindByEmailForResetPasswordAsync(model.Email);
                    if (hasData.Email != null)
                    {
                        var genrate_password = UtilityService.RandomPassword();
                        var resetPasswordDto = new ResetPasswordDto
                        {
                            Password = genrate_password.Trim(),
                            Email = model.Email,
                            PasswordChangedCount = hasData.PasswordChangedCount,
                            UserName = hasData.UserName
                        };

                        var isSetPassword = await _passwordEmailValidator.UpdatePasswordAsync(resetPasswordDto);
                        if (isSetPassword.Status)
                        {
                            string toEmail = model.Email;

                            // Get email settings
                            var emailSettings = await _passwordEmailValidator.GetEmailSettingAsync("Password");

                            // Enqueue  email
                            //_emailQueueService.Enqueue(new QueuedEmail<string>
                            //{
                            //    ToEmail = toEmail,
                            //    EmailSetting = emailSettings,
                            //    BodyFormatter = genrate_password => EmailTemplate.SendDefaultPasswordForgotPassword(resetPasswordDto.UserName.Trim(), genrate_password.Trim()),
                            //    Data = genrate_password.Trim(),
                            //    Flag = "credentials"
                            //});
                            return CustomResult("OTP verification successful. Please check your email.",
                                new { Status = isUpdated }, HttpStatusCode.OK);
                        }

                    }

                }

                return CustomResult("Failed to update SendOTPTime.", HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong.");
            }
        }


        [HttpPut, Route("EditUser")]
        public async Task<IActionResult> EditUserAsync([FromBody] EditUserRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _passwordEmailValidator.GetByIdAsync(model.UserID);
            if (user == null)
                return NotFound("User not found");

            var email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();

            user.EmployeeName = model.Name;
            user.Email = email;
            user.NormalizedEmail = email?.ToUpper();
            user.Address = model.Address;
            //user.IsActive = model.IsActive;
            user.UpdatedBy = user.Id.ToString();
            user.UpdatedDate = DateTime.UtcNow;

            var result = await _passwordEmailValidator.UpdateUserInfoAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                message = "User updated successfully",
                userId = user.Id
            });
        }


    }

}

