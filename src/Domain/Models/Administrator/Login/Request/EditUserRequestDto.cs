
using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Administrator.Login.Request
{
    public class EditUserRequestDto
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        //public bool IsActive { get; set; }
    }

}
