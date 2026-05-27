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
    [Required]
    [Range(1, 5, ErrorMessage = "Sunlight must be between 1 and 5")]
    public int SunlightIntensity { get; set; }
    [Required]
    [Range(1, 5, ErrorMessage = "Humidity must be between 1 and 5")]
    public int HumidityIntensity { get; set; }
}
