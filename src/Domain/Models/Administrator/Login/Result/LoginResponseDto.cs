namespace Domain.Models.Administrator.Login.Result
{
    public record LoginResponseDto(
      string Token,
      DateTime Expiration,
      long UserId,
      string Name,
      string PhoneNumber
  );
}
