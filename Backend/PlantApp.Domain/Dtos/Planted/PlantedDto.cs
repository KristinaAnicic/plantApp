using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.Planted;

public class PlantedDto
{
    public int Id { get; set; }
    public required string PlantName { get; set; }
    public required string Place {  get; set; }
    public ReferenceDto? PlantGroup { get; set; }
    public required string DatePlanted { get; set; }
    public string? PlantStatus { get; set; }
    public string? Image {  get; set; }
}
