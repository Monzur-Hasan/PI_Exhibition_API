using Domain.OtherModels.Pagination;

namespace Domain.Models.Administrator.Filter
{
    public class ApplicationRole_Filter : Sortparam
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? IsActive { get; set; }
        public string? ActivationStatus { get; set; }
        public string? StateStatus { get; set; }
        public string? IsApproved { get; set; }
        public string? Description { get; set; }
    }
}
