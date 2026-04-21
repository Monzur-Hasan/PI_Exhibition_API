
namespace Domain.Models.Administrator.DTO
{
    public class ProjectDto
    {
        public long Id { get; set; }   
        public long UserId { get; set; }
        public string Name { get; set; }   
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ProjectLocation { get; set; }
        public string ProjectMeasurement { get; set; }
        public string Problem { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();
    }
}
