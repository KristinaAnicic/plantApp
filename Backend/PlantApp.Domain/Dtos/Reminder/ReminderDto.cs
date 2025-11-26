namespace PlantApp.Domain.Dtos.Reminder;

public class ReminderDto
{
    public int Id { get; set; }
    public int PlantedId { get; set; }
    public required string Plant {  get; set; }
    public string? ReminderType { get; set; }
    public DateTime NextDueDate { get; set; }
    public string? Notes { get; set; }
    public bool IsLate { get; set; }
}
