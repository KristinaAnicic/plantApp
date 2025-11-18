using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Dtos.Reminder;

namespace PlantApp.Domain.Dtos.Planted;

public class PlantedGetDto
{
    public int Id { get; set; }
    public PlaceDto? Place { get; set; }
    public PlantDto? Plant { get; set; }
    public required DateTime DatePlanted { get; set; }
    public string? Source { get; set; }
    public string? Note { get; set; }
    public bool IsOutside { get; set; } = false;
    public string? PlantStatus { get; set; }
    public List<ReminderDto>? NextReminders { get; set; }
    public List<GrowthLogDto>? GrowthLogs { get; set; }
    public List<ImageDto>? Images { get; set; }
}
