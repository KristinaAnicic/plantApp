using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.Reminder;

namespace PlantApp.Domain.Dtos.PlantGroup;

public class PlantGroupGetDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<PlantedDto>? Planted { get; set; }
    public List<GrowthLogGetDto>? GrowthLogs { get; set; }
    public List<ReminderGetDto>? Reminders { get; set; }
}
