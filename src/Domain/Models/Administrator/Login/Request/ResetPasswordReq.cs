namespace Domain.Models.Administrator.Login.Request
{
    public class ResetPasswordReq
    {
        public string? UserName { get; set; }
        public string? CurrentPassword { get; set; }
        public string? Password { get; set; }
        public int PasswordChangedCount { get; set; }
    }

}
