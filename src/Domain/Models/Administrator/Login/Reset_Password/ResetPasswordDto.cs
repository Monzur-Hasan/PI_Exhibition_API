namespace Domain.Models.Administrator.Login.Reset_Password
{
    public class ResetPasswordDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int PasswordChangedCount { get; set; }
    }
}
