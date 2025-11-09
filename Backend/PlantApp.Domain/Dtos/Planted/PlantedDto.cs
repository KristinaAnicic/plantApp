using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.Planted;

public class PlantedDto
{
    public PlantDto? Plant { get; set; }
    public required DateTime DatePlanted { get; set; }
    public string? Source { get; set; }
    public string? Notes { get; set; }
    public bool IsOutside { get; set; } = false;
    public PlantStatus? PlantStatus { get; set; }
}
