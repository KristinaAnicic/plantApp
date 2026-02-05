using PlantApp.Domain.Dtos.Authentication;
using PlantBackend.ExceptionHandlers;

namespace PlantApp.Domain.Interfaces;

public interface IAuthService
{
    public Task<(TokenResponseDto?, ErrorResponse?)> LoginUser(LoginDto dto);
    public Task<TokenResponseDto> RefreshTokens(string token);
    public Task Logout(string token);
}
