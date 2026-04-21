using Domain.Models.Administrator.DTO;
using Domain.Models.Administrator.Login.Request;
using Domain.Models.Administrator.Login.Result;
using Domain.OtherModels.Response;

namespace Infrastructure.Features.Repository.Interface
{
    public interface IProjectRepository
    {
        Task<long> RegisterAsync(RegisterRequestDto request, List<string> imagePaths);
        Task<bool> CheckPhoneNumberExistsAsync(string phoneNumber);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<UserDto> GetUserByPhoneNumberAsync(string phoneNumber);
        Task SaveRefreshTokenAsync(long userId, string refreshToken, string deviceInfo);
        Task<UserRefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(long id);
        Task RevokeAllTokensAsync(long userId);
        Task<List<ProjectDto>> GetProjectsAsync();
        Task<ProjectDto> GetProjectByIdAsync(long id);
        Task<UserDto> GetuserByIdAsync(long id);
        Task UpdateProjectAsync(long id, UpdateProjectRequestDto request);
        Task DeleteProjectAsync(long id);

        Task<bool> UpdateUserAsync(UserDto user);

        Task<UpsertUserProjectResult> UpsertUserProjectAsync(RegisterRequestDto request, List<string> imagePaths);
    }
}
