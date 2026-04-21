using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Domain.Shared.Configurations;
using Domain.OtherModels.EmailService;
using Domain.OtherModels.Pagination;
using Domain.OtherModels.Response;

namespace Domain.Shared.Helpers
{
    public static class UtilityService
    {

        //public static string HashPassword(string password)
        //{
        //    if (string.IsNullOrEmpty(password))
        //        throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        //    using (var sha256 = SHA256.Create())
        //    {
        //        byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(hashedBytes);
        //    }
        //}

        //public static bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        //{
        //    var hashedPassword = HashPassword(enteredPassword); // Use the same HashPassword method
        //    if(hashedPassword == storedPasswordHash)
        //    {
        //        return true;
        //    }

        //    return false;          
        //}

        public static string GetPreviousMonthName(short monthNo)
        {
            if (monthNo >= 2 && monthNo <= 12)
            {
                return GetMonthName((short)(monthNo - 1));
            }
            else if (monthNo == 1)
            {
                return GetMonthName(12);
            }
            else
            {
                return "";
            }
        }

        public static int GetPreviousYear(short monthNo, int year)
        {
            if (monthNo >= 2 && monthNo <= 12)
            {
                return year;
            }
            else if (monthNo == 1)
            {
                return year - 1;
            }
            else
            {
                return -1;
            }
        }


        public static string GetFileMimetype(string fileExtension)
        {
            string mimetype = null;
            if (fileExtension.ToLower() == "pdf")
            {
                mimetype = Mimetype.PDF;
            }
            else if (fileExtension.ToLower() == "xls")
            {
                mimetype = Mimetype.EXCEL;
            }
            else if (fileExtension.ToLower() == "xlsx")
            {
                mimetype = Mimetype.EXCELX;
            }
            else if (fileExtension.ToLower() == "word")
            {
                mimetype = Mimetype.DOCX;
            }
            else if (fileExtension.ToLower() == "docx")
            {
                mimetype = Mimetype.DOCX;
            }
            else if (fileExtension.ToLower() == "doc")
            {
                mimetype = Mimetype.DOC;
            }
            else if (fileExtension.ToLower() == "png")
            {
                mimetype = Mimetype.PNG;
            }
            else if (fileExtension.ToLower() == "jpg" || fileExtension.ToLower() == "jpeg")
            {
                mimetype = Mimetype.JPEG;
            }
            return mimetype ?? Mimetype.PDF;
        }

        public static void AddDappperParams<T>(T Object, ref DynamicParameters parameters)
        {
            var keyValuePairs = GetKeyValuePairs(Object);
            foreach (var item in keyValuePairs)
            {
                parameters.Add(item.Key, item.Value);
            }
        }
        public static void AddDappperParams<T>(T Object, string[] excludeProps, ref DynamicParameters parameters)
        {
            var keyValuePairs = GetKeyValuePairs(Object);
            foreach (var item in keyValuePairs)
            {
                if (!excludeProps.Contains(item.Key))
                {

                    parameters.Add(item.Key, item.Value);
                }
            }
        }
        public static Pageparam GetPageparam(ref DynamicParameters parameters)
        {
            Pageparam pageparam = new Pageparam();
            pageparam.PageNumber = parameters.Get<short>("PageNumber");
            pageparam.PageSize = parameters.Get<short>("PageSize");
            pageparam.TotalPages = parameters.Get<int>("TotalPages");
            pageparam.TotalRows = parameters.Get<int>("TotalRows");
            return pageparam;
        }
        public static Dictionary<string, string> GetKeyValuePairs<T>(T obj, bool addBaseProperty = false)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            PropertyInfo[] infos = addBaseProperty == false ?
                obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) : obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                //var value = info.GetValue(obj, null) == null ? null : info.GetValue(obj, null).ToString().Trim();
                var value = info.GetValue(obj, null) == null ? null : info.GetValue(obj, null).ToString() == "null" ? null : info.GetValue(obj, null).ToString().Trim();
                keyValuePairs.Add(info.Name, value);
            }
            return keyValuePairs;
        }
        public static Dictionary<string, string> GetKeyValuePairs<T>(T obj, string[] excludeProps, bool addBaseProperty = false)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            PropertyInfo[] infos = addBaseProperty == false ?
                obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) : obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                var isInExcludeList = false;
                if (excludeProps != null)
                {
                    isInExcludeList = excludeProps.Contains(info.Name);
                }
                if (!isInExcludeList)
                {
                    //keyValuePairs.Add(info.Name, info.GetValue(obj, null) == null ? null : info.GetValue(obj, null).ToString().Trim());
                    keyValuePairs.Add(info.Name, info.GetValue(obj, null) == null ? null : info.GetValue(obj, null).ToString() == "null" ? null : info.GetValue(obj, null).ToString().Trim());
                }
            }
            return keyValuePairs;
        }


        public static string[] FileFormats = new string[] { "PDF", "CSV", "XLS" };
        public static string[] PageParams = new string[] { "PageSize", "PageNumber", "TotalRows", "TotalPages" };

        public static string ServiceEmailAddress = "yeasin@recombd.com";
        public static string ServiceEmailPassword = "";

        public static string[] numericCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        public static string[] smallAlphabetCharacters = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        public static string[] capitalAlphabetCharacters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "v", "W", "X", "Y", "Z" };

        public static string numericCharacter = "1234567890";
        public static string smallAlphabetCharacter = "abcdefghijklmnopqrstuvwxyz";
        public static string capitalAlphabetCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string specialCharacter = "!@#$%&*^";

        private static readonly Regex sWhitespace = new Regex(@"\s+");
        public static string ReplaceWhitespace(this string input)
        {
            string text = string.Empty;
            text = text.TrimEnd('\r', '\n');
            text = sWhitespace.Replace(input, " ");
            return text;
        }

        public static string GetEmailTemplate(string flag, string password = "")
        {
            if (flag == "ForgetPassword")
                return EmailTemplate.ForgotPasswordOTP(password);
            else if (flag == "DefaultPassword")
            {
                return EmailTemplate.SendDefaultPasswordForgotPassword("", password);
            }
            return string.Empty;
        }

        public static string GenerateRandomDigits(int length, string[] saAllowedCharacters)

        {
            string sOTP = string.Empty;
            string sTempChars = string.Empty;
            Random rand = new Random();
            for (int i = 0; i < length; i++)
            {
                int p = rand.Next(0, 5);
                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];
                sOTP += sTempChars;
            }
            return sOTP;
        }

        public static string GenerateRandomDigits(int length, string chars)
        {
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
        private const short maxPageSize = 50;
        public static int PageNumber(int pageNumber)
        {
            return pageNumber == 0 ? 1 : pageNumber;
        }
        //public static string RandomPassword()
        //{
        //    var thisDate = DateTime.Now;
        //    var random = (thisDate.Year + thisDate.Hour + thisDate.Minute).ToString() +
        //        (thisDate.Second + thisDate.Month + thisDate.Millisecond).ToString();

        //    return random;
        //}

        public static string RandomPassword()
        {
            var thisDate = DateTime.Now;
            // Year, Month, Day, Hour, Minute, Second, Millisecond by create generate password
            string part1 = thisDate.Millisecond.ToString("D3");  // 3 digit
            string part2 = thisDate.Second.ToString("D2");      // 2 digit
            string part3 = thisDate.Minute.ToString("D2");     // 2 digit
            string part4 = (thisDate.Hour % 10).ToString();     // 1 digit
            string password = part1 + part2 + part3 + part4;
            // Ensure it's exactly 8 digits
            return password.Length >= 8 ? password.Substring(0, 8) : password.PadLeft(8, '0');
        }


        public static string ShortRandomPassword()
        {          
            int random = new Random().Next(100000, 999999);
            return random.ToString();
        }

        public static int PageSize(int pageSize)
        {
            return pageSize > maxPageSize ? maxPageSize : pageSize;
        }
        public static string JsonData(object obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static string JsonData<T>(IEnumerable<T> obj)
        {
            JsonSerializerOptions json = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // to make property in camel case
            };
            json.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;  // To prevent encoding of bangla font/text
            var jsonData = JsonSerializer.Serialize(obj, json);
            return jsonData;
        }
        public static string JsonData<T>(T obj)
        {
            JsonSerializerOptions json = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase // to make property in camel case
            };
            json.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;  // To prevent encoding of bangla font/text
            var jsonData = JsonSerializer.Serialize(obj, json);
            return jsonData;
        }
        public static T JsonToObject<T>(string json)
        {
            try
            {
                if (json != null)
                {
                    var options = new JsonSerializerOptions()
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString |
                    JsonNumberHandling.WriteAsString
                    };
                    var data = JsonSerializer.Deserialize<T>(json, options);
                    return data;
                }
            }
            catch (Exception)
            {
            }
            return (T)Convert.ChangeType(null, typeof(T));
        }
        public static IEnumerable<T> JsonToObject<T>(string json, string text)
        {
            if (json != null)
            {
                var options = new JsonSerializerOptions() { NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString };
                var data = JsonSerializer.Deserialize<IEnumerable<T>>(json, options);
                return data;
            }
            return new List<T>();
        }
        public static string GetImage(string filePath)
        {
            string base64Img = string.Empty;
            if (File.Exists(filePath))
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                BinaryReader br = new BinaryReader(fs);

                byte[] bytes = br.ReadBytes((int)fs.Length);

                br.Close();
                string extension = Path.GetExtension(filePath);

                var base64 = Convert.ToBase64String(bytes);
                base64Img = string.Format("data:image/{0};base64,{1}", extension, base64);
            }
            return base64Img;
        }
        public static string GetMonthName(short monthNo)
        {
            switch (monthNo)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return "";
            }
        }
        public static async Task<string> SaveFileAsync(IFormFile file, string subfolderName)
        {
            var uniqueFileName = Guid.NewGuid().ToString();
            var extension = GetFileExtension(file);
            var fileFolder = GetFileFolder(extension.ToLower());
            uniqueFileName = uniqueFileName + extension.ToLower();
            var path = GetDriverPath(subfolderName, fileFolder); // string.Format(@"{0}/{1}/{2}", PhysicalDocFolder, subfolderName, fileFolder);
            var driverPath = PhysicalDriver + "/" + path;
            using (var stream = new FileStream(Path.Combine(string.Format(@"{0}", driverPath), uniqueFileName), FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return string.Format(@"{0}/{1}", path, uniqueFileName);
        }

        public static string PhysicalDriver = ConfigurationHelper.config.GetSection("PhysicalDriver").Value.ToString();
        public static string PhysicalDocFolder = "RentalServiceDocs";
        public static string PhysicalImgFolder = "RentalServiceImages";

        /// <summary>
        /// Return Image Directory
        /// </summary>
        /// <param name="orgCode"></param>
        /// <returns></returns>
        public static void CreateOrgDirectory(string orgCode)
        {
            string docPath = string.Format(@"{0}/{1}/{2}", PhysicalDriver, PhysicalDocFolder, orgCode);
            Directory.CreateDirectory(docPath);

            Directory.CreateDirectory(string.Format(@"{0}/{1}", docPath, "Pdf"));
            Directory.CreateDirectory(string.Format(@"{0}/{1}", docPath, "Excel"));
            Directory.CreateDirectory(string.Format(@"{0}/{1}", docPath, "Images"));

            string imgPath = string.Format(@"{0}/{1}/{2}", PhysicalDriver, PhysicalImgFolder, orgCode);
            Directory.CreateDirectory(imgPath);

            Directory.CreateDirectory(string.Format(@"{0}/{1}", imgPath, "Images"));
        }
        public static string PhysicalDriverFilePath
        {
            get
            {
                return string.Format(@"{0}/{1}/", PhysicalDriver, PhysicalDocFolder);
            }
        }
        public static string GetFileFolder(string fileExtension)
        {
            string fileFolder = string.Empty;
            if (fileExtension == ".jpeg" || fileExtension == ".jpg" || fileExtension == ".png")
            {
                fileFolder = "Images";
            }
            else if (fileExtension == ".pdf")
            {
                fileFolder = "Pdf";
            }
            else if (fileExtension == ".xlsx" || fileExtension == ".xls")
            {
                fileFolder = "Excel";
            }
            return fileFolder;
        }
        public static string GetDriverPath(string subfolder, string fileFloder)
        {
            string path = string.Empty;
            if (fileFloder == "Images")
            {
                path = string.Format(@"{0}/{1}/{2}", PhysicalImgFolder, subfolder, fileFloder);
            }
            else if (fileFloder != "Images")
            {
                path = string.Format(@"{0}/{1}/{2}", PhysicalDocFolder, subfolder, fileFloder);
            }
            return path;
        }
        public static string GetFileExtension(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            return fileExtension;
        }

        /// <summary>
        /// Find the file extension from a path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        public static string GetFileExtension(string file)
        {
            var fileExtension = file.Substring(file.LastIndexOf(".") + 1);
            return fileExtension;
        }

        /// <summary>
        /// Find the file name from a path
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>

        public static string GetFileName(string file)
        {
            var fileName = file.Substring(file.LastIndexOf("/") + 1);
            return fileName;
        }

        public static int GetMonthDiffExcludingThisMonth(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }
        public static int GetMonthDiffIncludingThisMonth(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart) + 1;
        }
        public static double GetDateDiff(DateTime startDate, DateTime endDate)
        {
            double days = Math.Round((endDate - startDate).TotalDays + 1);
            return days;
        }
        public static double GetHoursDiff(TimeSpan startTime, TimeSpan endTime)
        {
            double hours = (endTime - startTime).TotalHours;
            return hours;
        }
        public static string PhysicalDriverImagePath
        {
            get
            {
                return @"C:/Projects/Images/";
            }
        }
        /// <summary>
        /// Checking a string with null,empty and whitespace
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullEmptyOrWhiteSpace(string value)
        {
            return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? true : false;

        }
        public static void DeleteFile(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
        }
        public static byte[] GetFileBytes(string filepath)
        {
            byte[] fileByte = null;
            if (File.Exists(filepath))
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fileByte = bytes;
            }
            return fileByte;
        }
        public static string ParamChecker(string param)
        {
            if (param.ToLower().Contains(";") || param.ToLower().Contains("delete") || param.ToLower().Contains("database") || param.ToLower().Contains("alter") || param.ToLower().Contains("truncate") || param.ToLower().Contains("column") || param.ToLower().Contains("drop"))
            {
                return param = "";
            }
            return param;
        }
        public static List<string> ExtensionFlags()
        {
            return new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        }
        public static ExecutionStatus Invalid(string message = "")
        {
            message = string.IsNullOrEmpty(message) || string.IsNullOrWhiteSpace(message) ? "Invalid Execution" : message;
            return new ExecutionStatus() { Status = false, Msg = message };
        }
        public static List<string> ListOfStateStatus() =>
            new List<string> {
                StateStatus.Entered,
                StateStatus.Approved,
                StateStatus.Recheck,
                StateStatus.Pending,
                StateStatus.Checked,
                StateStatus.Cancelled
            };
        public static long TryParseLong(string value)
        {
            long result = 0;
            long.TryParse(value, out result);
            return result;
        }
        public static int TryParseInt(string value)
        {
            int result = 0;
            int.TryParse(value, out result);
            return result;
        }

        public static decimal TryParseDecimal(string value)
        {
            decimal result = 0;
            decimal.TryParse(value, out result);
            return result;
        }
        /// <summary>
        /// Unformated string to propercase
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static string ProperCase(string title)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            title = textInfo.ToTitleCase(title);
            return title;
        }
        /// <summary>
        /// Checking If the receivedStatus exist within the array of string status
        /// </summary>
        /// <param name="receivedStatus"></param>
        /// <param name="selectedStatus"></param>
        /// <returns></returns>
        public static bool StatusChecking(string receivedStatus, string[] selectedStatus)
        {
            if (selectedStatus.Length == 0)
                throw new Exception("Array length is 0");
            return selectedStatus.FirstOrDefault(f => f == receivedStatus) != null;
        }

        public static class GoogleAuthKeys
        {
            public static string GoogleAuthKey
            {
                get
                {
                    return "U1d2A3y4";
                }
            }
        }




        public static Dictionary<string, dynamic> GetKeyValuePairsDynamic<T>(T obj, bool addBaseProperty = false)
        {
            Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>();
            PropertyInfo[] infos = addBaseProperty == false ?
                obj.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public) : obj.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                var value = info.GetValue(obj, null) == null ? null : info.GetValue(obj, null).ToString();
                keyValuePairs.Add(info.Name, value);
            }
            return keyValuePairs;
        }
        public static string GenerateSelectQuery(string tableName)
        {
            return $"SELECT * FROM {tableName} ";
        }
        public static string GenerateInsertQuery(string tableName, List<string> paramkeys, string output = "")
        {
            string columns = string.Join(",", paramkeys);
            string values = string.Join(",", paramkeys.Select(x => "@" + x));
            string query = $"INSERT INTO {tableName} ({columns}) {output} VALUES ({values})";
            return query;

            //return "INSERT INTO Payroll_OvertimePolicy (col1, col2) OUTPUT INSERTED.Id VALUES (@val1, @val2)";
        }
        public static string GenerateUpdateQuery(string tableName, List<string> paramkeys)
        {
            List<string> columnValues = new();
            foreach (var key in paramkeys)
            {
                columnValues.Add($"{key} = @{key}");
            }

            string query = $"UPDATE {tableName} SET {string.Join(", ", columnValues)} ";
            return query;

            //UPDATE table_name
            //SET column1 = value1, column2 = value2, ...
            //WHERE condition;
        }
        public static string GenerateDeleteQuery(string tableName)
        {
            return $"DELETE  FROM {tableName} ";

        }
    

        public static async Task<bool> EmailService<T>(string toEmail, EmailSettingObject emailSetting, Func<T, string> bodyFormatter, T data, string flag = "")
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(emailSetting.DisplayName, emailSetting.EmailAddress));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = emailSetting.Subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailSetting.IsBodyHtml ? bodyFormatter(data) : null,
                    TextBody = !emailSetting.IsBodyHtml ? bodyFormatter(data) : null
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                int port = int.Parse(emailSetting.Port);

                // Determine correct socket option based on port
                SecureSocketOptions socketOptions = port switch
                {
                    465 => SecureSocketOptions.SslOnConnect,
                    587 => SecureSocketOptions.StartTls,
                    _ => SecureSocketOptions.Auto
                };

                await client.ConnectAsync(emailSetting.Host, port, socketOptions);

                if (!emailSetting.UseDefaultCredentials)
                {
                    await client.AuthenticateAsync(emailSetting.EmailAddress, emailSetting.EmailPassword);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"MailKit error: {ex}");
                return false;
            }
        }
    
        //public static async Task<bool> EmailService<T>(string toEmail, EmailSettingObject emailSetting, Func<T, string> bodyFormatter, T data, string flag = "")
        //{
        //    try
        //    {
        //        using (MailMessage message = new MailMessage())
        //        {
        //            message.From = new MailAddress(emailSetting.EmailAddress, emailSetting.DisplayName);
        //            message.To.Add(new MailAddress(toEmail));
        //            message.Subject = emailSetting.Subject;
        //            message.IsBodyHtml = emailSetting.IsBodyHtml;
        //            message.Body = bodyFormatter(data);

        //            using (SmtpClient smtp = new SmtpClient())
        //            {
        //                smtp.EnableSsl = emailSetting.EnableSsl;
        //                smtp.UseDefaultCredentials = emailSetting.UseDefaultCredentials;
        //                smtp.Port = Convert.ToInt32(emailSetting.Port);
        //                smtp.Host = emailSetting.Host;
        //                smtp.Credentials = new NetworkCredential(emailSetting.EmailAddress, emailSetting.EmailPassword);
        //                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

        //                await smtp.SendMailAsync(message);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Unexpected error while sending email: {ex.Message}");
        //    }

        //    return false;
        //}
        //public static async Task<bool> EmailService(string toEmail, string password, EmailSettingObject emailSetting, AgencyRegistrationDto dto, string flag = "")
        //{
        //    try
        //    {
        //        using (MailMessage message = new MailMessage())
        //        {
        //            message.From = new MailAddress(emailSetting.EmailAddress, emailSetting.DisplayName);
        //            message.To.Add(new MailAddress(toEmail));
        //            message.Subject = emailSetting.Subject;
        //            message.IsBodyHtml = emailSetting.IsBodyHtml;
        //            message.Body = EmailTemplate.AgencyRegistrationDetails(dto);

        //            using (SmtpClient smtp = new SmtpClient())
        //            {
        //                smtp.EnableSsl = emailSetting.EnableSsl;
        //                smtp.UseDefaultCredentials = emailSetting.UseDefaultCredentials;
        //                smtp.Port = Convert.ToInt32(emailSetting.Port);
        //                smtp.Host = emailSetting.Host;
        //                smtp.Credentials = new NetworkCredential(emailSetting.EmailAddress, emailSetting.EmailPassword);
        //                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

        //                await smtp.SendMailAsync(message);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Unexpected error while sending email: {ex.Message}");
        //    }

        //    return false;
        //}


    }
}

