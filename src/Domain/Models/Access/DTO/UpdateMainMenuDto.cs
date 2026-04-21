namespace Domain.Models.Access.DTO
{
    public class UpdateMainMenuDto
    {
        public string MMId { get; set; }
        public string? MenuName { get; set; }
        public string? ShortName { get; set; }
        public string? IconClass { get; set; }
        public string? IconColor { get; set; }
        public long? ApplicationId { get; set; }
        public bool? IsActive { get; set; }
        public int? SequenceNo { get; set; }
        public string? ServiceID { get; set; }
    }
}
