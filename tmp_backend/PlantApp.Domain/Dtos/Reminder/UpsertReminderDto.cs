using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.Reminder;

public class UpsertReminderDto
{
    public int Id { get; set; }
    [Required]
    public int PlantedId { get; set; }
    [Required]
    public required int ReminderTypeId { get; set; }
    [Required]
    public required int FrequencyTypeId { get; set; }
    [Required]
    public required int FrequencyNum { get; set; }
    [Required]
    public DateTime OriginalDueDate { get; set; }
    public string? Note { get; set; }
}
