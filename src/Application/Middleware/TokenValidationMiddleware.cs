using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Middleware
{
    using Domain.Shared.Settings;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using System.Threading.Tasks;

    namespace Application.Middleware
    {
        public class TokenValidationMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<TokenValidationMiddleware> _logger;

            public TokenValidationMiddleware(RequestDelegate next, ILogger<TokenValidationMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            //public async Task InvokeAsync(HttpContext context)
            //{
            //    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        try
            //        {
            //            var key = AppSettings.GetSymmetricSecurityKey();
            //            var tokenHandler = new JwtSecurityTokenHandler();

            //            tokenHandler.ValidateToken(token, new TokenValidationParameters
            //            {
            //                ValidateIssuer = true,
            //                ValidateAudience = true,
            //                ValidateIssuerSigningKey = true,
            //                ValidateLifetime = true,
            //                ValidIssuer = AppSettings.JwtIssuer,
            //                ValidAudience = AppSettings.JwtAudience,
            //                IssuerSigningKey = new SymmetricSecurityKey(key),
            //                ClockSkew = TimeSpan.Zero
            //            }, out SecurityToken validatedToken);
            //        }
            //        catch (Exception ex)
            //        {
            //            _logger.LogWarning("Invalid token: " + ex.Message);
            //            context.Response.StatusCode = 401; // Unauthorized
            //            await context.Response.WriteAsync("Invalid or expired token");
            //            return;
            //        }
            //    }

            //    await _next(context);
            //}
        }

        // Extension method
        public static class TokenValidationMiddlewareExtensions
        {
            public static IApplicationBuilder UseTokenValidation(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<TokenValidationMiddleware>();
            }
        }
    }
}
