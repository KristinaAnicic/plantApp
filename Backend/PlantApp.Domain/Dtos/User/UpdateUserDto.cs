using PlantApp.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Dtos.User;

public class UpdateUserDto
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public string? Contact { get; set; }
    public char Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
}
