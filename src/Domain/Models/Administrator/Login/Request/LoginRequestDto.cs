namespace Domain.Models.Administrator.Login.Request
{
    public record LoginRequestDto(       
        string phoneNumber = "", bool rememberMe = false
    );   
}
