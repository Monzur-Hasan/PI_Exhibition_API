namespace Domain.OtherModels.Report
{
    public class ReportFile
    {
        public byte[] FileBytes { get; set; }
        public string Mimetype { get; set; }
        public string Extension { get; set; }
        public string FileName { get; set; }
    }
}
