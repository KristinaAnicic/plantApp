using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class Reminder : BaseEntity
{
    [ForeignKey(nameof(PlantedId))]
    public Planted? Planted {  get; set; }
    public int PlantedId { get; set; }
    [ForeignKey(nameof(ReminderTypeId))]
    public ReminderType? ReminderType { get; set; }
    public int ReminderTypeId { get; set; }
    public required string Frequency { get; set; }
    public DateTime NexDueDate { get; set; }
    public string? Notes { get; set; }
}
