using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Dtos.Reminder;

namespace PlantApp.Domain.Dtos.Planted;

public class PlantedGetDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PlaceDto? Place { get; set; }
    public PlantDto? Plant { get; set; }
    public required DateOnly DatePlanted { get; set; }
    public required string DatePlantedString { get; set; }
    public required string LastUpdate { get; set; }
    public string? Source { get; set; }
    public string? Note { get; set; }
    public bool IsOutside { get; set; } = false;
    public int? UserId { get; set; }
    public ReferenceDto? PlantStatus { get; set; }
    public string? Image { get; set; }
    public List<ReminderGetDto>? NextReminders { get; set; }
    public List<GrowthLogGetDto>? GrowthLogs { get; set; }
    public List<ImageDto>? Images { get; set; }
}
