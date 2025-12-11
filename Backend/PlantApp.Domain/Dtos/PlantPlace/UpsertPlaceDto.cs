using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.PlantPlace;

public class UpsertPlaceDto
{
    public int? Id { get; set; }

    [Required]
    public required string Name { get; set; }
    public string? Address { get; set; }
    [Required]
    public required string City { get; set; }
    public string? Note { get; set; }
    [Required]
    public int CountryId { get; set; }
}
