using PlantApp.Domain.Models.Categories;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class Reminder : BaseEntity
{
    [ForeignKey(nameof(PlantedId))]
    public Planted? Planted {  get; set; }
    public int PlantedId { get; set; }

    [ForeignKey(nameof(ReminderTypeId))]
    public ReminderType? ReminderType { get; set; }
    public int ReminderTypeId { get; set; }

    [ForeignKey(nameof(FrequencyTypeId))]
    public Frequency? FrequencyType { get; set; }
    public required int FrequencyTypeId { get; set; }
    public required int FrequencyNum { get; set; }
    public DateTime OriginalDueDate { get; set; }
    public DateTime NextDueDate { get; set; }
    public string? Note { get; set; }
}
