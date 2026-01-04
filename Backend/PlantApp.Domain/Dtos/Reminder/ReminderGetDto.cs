using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.Reminder;

public class ReminderGetDto
{
    public int Id { get; set; }
    public string? PlantedName { get; set; }
    public int PlantedId { get; set; }  
    public ReferenceDto? ReminderType { get; set; }
    public ReferenceDto? FrequencyType { get; set; }
    public DateTime NextDueDate { get; set; }
    public int DaysDelayed { get; set; }
    public int FrequencyNum { get; set; }
    public string? Notes { get; set; }
    public bool IsLate { get; set; }
}
