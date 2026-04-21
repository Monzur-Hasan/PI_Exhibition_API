
using Domain.Models.Administrator.DTO;

namespace Infrastructure.Features.Service.Token_Service
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(UserDto user);
        string GenerateRefreshToken();
    }
}
