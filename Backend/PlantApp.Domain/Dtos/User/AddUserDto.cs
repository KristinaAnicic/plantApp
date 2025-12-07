using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.User;

public class AddUserDto : UpdateUserDto
{
    [Required]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public required string Email { get; set; }
    public required string Username { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password must be longer than 8 characters")]
    public required string Password { get; set; }
}
