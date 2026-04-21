namespace Domain.OtherModels.Report
{
    public class ReportLayer
    {
        public long OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public byte[] OrgPic { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public byte[] CompanyPic { get; set; }
        public long DivisionId { get; set; }
        public string DivisionName { get; set; }
        public long BranchId { get; set; }
        public string BranchName { get; set; }
        public byte[] ReportLogo { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string HeaderText { get; set; }
        public string FooterText { get; set; }
        public string ReportLogoPath { get; set; }
        public string CompanyLogoPath { get; set; }
        public string ShortName { get; set; }
        public string Username { get; set; }
        public string EmployeeName { get; set; }
        public string BranchLogoPath { get; set; }
        public string IssueDate { get; set; }
        public byte[] BranchLogo { get; set; }
    }

}
