namespace Domain.Models.Administrator.Login.Request
{
    public class ValidateTokenRequestDto
    {
        public string? PhoneNumber { get; set; }
        public string? Token { get; set; }
    }
}
