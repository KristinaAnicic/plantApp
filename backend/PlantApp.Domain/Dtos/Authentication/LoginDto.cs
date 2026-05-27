using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.Authentication;

public class LoginDto
{
    [Required]
    public required string UsernameOrEmail { get; set; }
    [Required]
    public required string Password { get; set; }
}
