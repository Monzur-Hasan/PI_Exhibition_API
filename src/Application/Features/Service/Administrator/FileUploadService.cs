using Microsoft.AspNetCore.Http;
using Domain.Models.Common;

namespace Application.Features.Service.Administrator
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _basePath;

        public FileUploadService(string basePath)
        {
            _basePath = basePath;
        }

        //public async Task<string> UploadFileAsync(IFormFile file, string subDirectory)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        // Create the directory if it doesn't exist
        //        var directoryPath = Path.Combine(_basePath, subDirectory);
        //        if (!Directory.Exists(directoryPath))
        //        {
        //            Directory.CreateDirectory(directoryPath);
        //        }

        //        // Generate a unique file name
        //        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        //        var filePath = Path.Combine(directoryPath, fileName);

        //        // Save the file
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        // Return the file path or URL
        //        return filePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while uploading the file.", ex);
        //    }

        //}
        public async Task<UploadedFileInfo> UploadFileAsync(IFormFile file, string subDirectory)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            try
            {
                // Create the directory if it doesn't exist
                var directoryPath = Path.Combine(_basePath, subDirectory);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Get the file extension
                var fileExtension = Path.GetExtension(file.FileName);

                // Generate a unique file name with the extension
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Calculate file size in KB and MB
                double fileSizeInKB = file.Length / 1024.0;
                double fileSizeInMB = fileSizeInKB / 1024.0;
                string fileSizeWithUnit = fileSizeInMB >= 1
                    ? fileSizeInMB.ToString("F2") + " MB"
                    : fileSizeInKB.ToString("F2") + " KB";

                // Return file details as an object
                return new UploadedFileInfo
                {
                    FileName = fileName,
                    ActualFileName = file.FileName,
                    FileSize = fileSizeWithUnit,
                    FilePath = filePath,
                    FileType = fileExtension
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file.", ex);
            }
        }

        public async Task<UploadedFileInfo> UploadNewFileAsync(IFormFile file, string subDirectory)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null.");
            }

            try
            {
                // Create the directory if it doesn't exist
                var directoryPath = Path.Combine(_basePath, subDirectory);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Get the file extension
                var fileExtension = Path.GetExtension(file.FileName);

                // Generate a unique file name with the extension
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(directoryPath, fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Calculate file size in KB and MB
                double fileSizeInKB = file.Length / 1024.0;
                double fileSizeInMB = fileSizeInKB / 1024.0;
                string fileSizeWithUnit = fileSizeInMB >= 1
                    ? fileSizeInMB.ToString("F2") + " MB"
                    : fileSizeInKB.ToString("F2") + " KB";

                // Create the new file endpoint path in the format "/new-subfolder/filepath"
                var newFilePathEndpoint = $"/{subDirectory}/{fileName}";

                // Return file details as an object with the new endpoint path
                return new UploadedFileInfo
                {
                    FileName = fileName,
                    ActualFileName = file.FileName,
                    FileSize = fileSizeWithUnit,
                    FilePath = newFilePathEndpoint, // Using the new endpoint format
                    FileType = fileExtension
                };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while uploading the file.", ex);
            }
        }
    }
}
