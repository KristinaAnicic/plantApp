using PlantApp.Domain.Dtos.Authentication;

namespace PlantApp.Domain.Interfaces;

public interface IAuthService
{
    public Task<TokenResponseDto> LoginUser(LoginDto dto);
    public Task<TokenResponseDto> RefreshTokens(RefreshTokenRequestDto request);
    public Task Logout(RefreshTokenRequestDto dto);
}
