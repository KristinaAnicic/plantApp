using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Dtos.PlantPlace;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Dtos.User;

public class UserGetDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public string? Role { get; set; }
    public char Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public double? Rating { get; set; }
    public int? NumOfRatings { get; set; }
    public List<PlaceGetDto>? Places { get; set; }
    public List<PlantExchangeDto>? PlantExchanges { get; set; }
}
