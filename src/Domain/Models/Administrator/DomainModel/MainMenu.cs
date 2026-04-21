namespace Domain.Models.Administrator.DomainModel
{
    public class MainMenu
    {
        public string MMId { get; set; }
        public string MenuName { get; set; }
        public string ShortName { get; set; }
        public string IconClass { get; set; }
        public string IconColor { get; set; }
        public long MId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long ApplicationId { get; set; }
        public bool IsActive { get; set; }
        public int SequenceNo { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string ServiceID { get; set; }
    }
}