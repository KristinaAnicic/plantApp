namespace Plant.Data.Models;

public class ReminderType : BaseEntity
{
    public required string Type { get; set; }
    public ICollection<Reminder>? Reminders { get; set; }
}
