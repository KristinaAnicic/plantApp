using PlantApp.Domain.Dtos.User;

namespace PlantApp.Domain.Dtos.Authentication;

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required UserDto User { get; set; }
}
