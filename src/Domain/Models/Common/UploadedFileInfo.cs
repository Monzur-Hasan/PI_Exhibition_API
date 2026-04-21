using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Common
{
    public class UploadedFileInfo
    {
        public string FileName { get; set; } // Unique file name
        public string ActualFileName { get; set; } // Original file name
        public string FileSize { get; set; } // File size with unit (KB or MB)
        public string FilePath { get; set; } // Full file path
        public string FileType { get; set; } // File extension
    }
}
