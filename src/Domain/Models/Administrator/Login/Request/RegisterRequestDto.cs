using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Administrator.Login.Request
{
    public class RegisterRequestDto
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; } // optional

        public string? Password { get; set; }

        [Required]
        public string ProjectLocation { get; set; }

        [Required]
        public string ProjectMeasurement { get; set; }

        [Required]
        public string Problem { get; set; }
        public string? Comments { get; set; }

       
        public string? Recaptcha { get; set; }
        public List<IFormFile>? ProjectImages { get; set; }
       
    }

}
