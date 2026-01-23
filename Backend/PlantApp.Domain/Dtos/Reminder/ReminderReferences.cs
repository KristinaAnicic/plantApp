using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.Reminder;

public class ReminderReferences
{
    public List<ReferenceDto> ReminderTypes { get; set; } = new List<ReferenceDto>();
    public List<ReferenceDto> FrequencyTypes { get; set; } = new List<ReferenceDto>();
}
