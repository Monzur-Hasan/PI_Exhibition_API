using Domain.Models.Common;

namespace Domain.Models.Administrator.DTO
{
    public class UpdateRoleRequestDto : BaseModel
    {
        public string? Id { get; set; }      
        public string? AgencyID { get; set; }      
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
