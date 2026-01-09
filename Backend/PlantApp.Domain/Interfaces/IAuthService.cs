using PlantApp.Domain.Dtos.Authentication;

namespace PlantApp.Domain.Interfaces;

public interface IAuthService
{
    public Task<(string, TokenResponseDto)> LoginUser(LoginDto dto);
    public Task<(string, TokenResponseDto)> RefreshTokens(string token);
    public Task Logout(string token);
}
