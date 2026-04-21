using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Administrator.DTO
{
    public class UpdateProjectRequestDto
    {
        public string ProjectLocation { get; set; }
        public string ProjectMeasurement { get; set; }
        public string Problem { get; set; }
        public string Comments { get; set; }

        public List<IFormFile> NewImages { get; set; } // new upload
        public List<long> DeleteImageIds { get; set; } // remove existing

    }
}
