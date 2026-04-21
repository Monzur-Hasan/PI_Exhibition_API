namespace Domain.OtherModels.DataService
{
    public class Select2Dropdown
    {
        public string? Code { get; set; }
        public string? Id { get; set; }
        public string? Value { get; set; }
        public string? Text { get; set; }
        public bool? Disabled { get; set; }
        public bool? IsFixed { get; set; }
        public bool? IsActive { get; set; }
        public string? Type { get; set; }
        public string? Flag { get; set; }
        public string? Count { get; set; }
        public string? Max { get; set; }
        public string? Duration { get; set; }
    }

    public class SelectDropdownMediatR
    { 
        public string? Id { get; set; }
        public string? Text { get; set; }
    }

}
