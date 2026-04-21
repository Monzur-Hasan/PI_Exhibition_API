using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Domain.Shared.ValidationFilters
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly string[] _imageOrPdfExtensions = new string[] { ".jpeg", ".jpg", ".png", ".pdf" };
        private readonly string[] _excelExtensions = new string[] { ".xlsx" };
        private long _maxSize = 0; // Size In KB
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        public AllowedExtensionsAttribute(string[] extensions, long maxSize = 0)
        {
            _extensions = extensions;
            _maxSize = maxSize > 0 ? maxSize * 1024 : 0;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            long fileSize = 0;
            if (file != null)
            {
                fileSize = file.Length;
                var extension = Path.GetExtension(file.FileName);
                var validExtensions = "";
                _extensions.ToList().ForEach(item => validExtensions += item.Substring(1) + ",");
                if (_extensions.Contains(extension.ToLower()))
                {
                    if (!IsValidFileContent(file, extension))
                    {
                        //return new ValidationResult(GetErrorMessage(validExtensions.Substring(0, validExtensions.Length - 1)));
                        return new ValidationResult("Invalid file extention");
                    }
                }
                else
                {
                    return new ValidationResult("Invalid file extention");
                }

                if (_maxSize > 0)
                {
                    if (fileSize > _maxSize)
                    {
                        return new ValidationResult("File size is greater than " + _maxSize.ToString() + " KB");
                    }
                }
            }
            return ValidationResult.Success;
        }
        private string GetErrorMessage(string validtype)
        {
            return $"Your filetype is not valid. Require: {validtype}";
        }

        bool IsValidFileExtension(string fileExtension)
        {
            // List of allowed file extensions
            string[] allowedExtensions = { ".jpeg", ".jpg", ".png", ".pdf" };

            // Convert file extension to lower case for case-insensitive comparison
            string lowerCaseExtension = fileExtension.ToLower();

            // Check if the file extension is in the list of allowed extensions
            return allowedExtensions.Contains(lowerCaseExtension);
        }

        // Method to validate file Content
        bool IsValidFileContent(IFormFile file, string fileExtension)
        {
            try
            {
                // Check if the file is an image (JPEG, PNG, JPG) or a PDF
                MemoryStream fileStream = new MemoryStream();
                file.CopyTo(fileStream);
                fileStream.Position = 0;
                if (_imageOrPdfExtensions.Contains(fileExtension))
                {

                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        var headerBytes = binaryReader.ReadBytes(4);
                        if (headerBytes.Length >= 4)
                        {
                            if (headerBytes[0] == 0x25 && headerBytes[1] == 0x50 && headerBytes[2] == 0x44 && headerBytes[3] == 0x46)
                            {
                                // PDF file
                                return true;
                            }
                            else if (headerBytes[0] == 0xFF && headerBytes[1] == 0xD8 && headerBytes[2] == 0xFF && (headerBytes[3] == 0xE0 || headerBytes[3] == 0xE1))
                            {
                                // JPEG file
                                return true;
                            }
                            else if (headerBytes[0] == 0x89 && headerBytes[1] == 0x50 && headerBytes[2] == 0x4E && headerBytes[3] == 0x47)
                            {
                                // PNG file
                                return true;
                            }
                        }
                    }
                }
                // Check if the file is EXCEL
                else if (_excelExtensions.Contains(fileExtension))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        var headerBytes = binaryReader.ReadBytes(4);
                        if (headerBytes.Length >= 4)
                        {
                            // XLSX file signature
                            if (headerBytes[0] == 0x50 && headerBytes[1] == 0x4B && headerBytes[2] == 0x03 && headerBytes[3] == 0x04)
                            {
                                return true;
                            }
                            // XLS file signature
                            else if (headerBytes[0] == 0xD0 && headerBytes[1] == 0xCF && headerBytes[2] == 0x11 && headerBytes[3] == 0xE0)
                            {
                                return true;
                            }
                        }
                    }
                }
                // If the file format is not supported
                return false;
            }
            catch
            {
                // Content validation failed
                return false;
            }
        }

    }
}
