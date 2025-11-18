namespace PlantApp.Domain.Dtos.Reminder;

public class ReminderGetDto : ReminderDto
{
    public string? PlantedName { get; set; }
    public string? Place { get; set; }
    public required string Frequency { get; set; }
}
