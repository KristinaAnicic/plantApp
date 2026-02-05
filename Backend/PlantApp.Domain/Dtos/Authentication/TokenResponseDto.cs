using PlantApp.Domain.Dtos.User;
using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.Authentication;

public class TokenResponseDto
{
    public required string AccessToken { get; set; }
    public required UserDto User { get; set; }

    [JsonIgnore]
    public string? RefreshToken { get; set; }
}
