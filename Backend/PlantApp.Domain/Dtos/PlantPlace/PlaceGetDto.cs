using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.PlantPlace;

public class PlaceGetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public required ReferenceDto Country { get; set; }
    public string? Note { get; set; }
    public List<PlantedDto>? Planted { get; set; }
    public int SunlightIntensity { get; set; }
    public int HumidityIntensity { get; set; }
}
