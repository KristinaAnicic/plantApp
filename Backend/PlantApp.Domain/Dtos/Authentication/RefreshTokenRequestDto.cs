using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.Authentication;

public class RefreshTokenRequestDto
{
    public int UserId { get; set; }
    [Required]
    public required string RefreshToken { get; set; }
}
