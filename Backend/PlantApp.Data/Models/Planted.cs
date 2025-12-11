using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Planted : BaseEntity, IHasImages
{
    [ForeignKey(nameof(PlaceId))]
    public Place? Place { get; set; }
    public required int PlaceId { get; set; }

    [ForeignKey(nameof(PlantId))]
    public Plant? Plant { get; set; }
    public required int PlantId { get; set; }

    public string? Name { get; set; }
    public required DateTime DatePlanted { get; set; }
    public string? Source {  get; set; }
    public string? Note { get; set; }
    public bool IsOutside { get; set; } = false;
    public string? Image { get; set; }

    [ForeignKey(nameof(PlantStatusId))]
    public PlantStatus? PlantStatus { get; set; }
    public int PlantStatusId { get; set; }

    public ICollection<GrowthLog>? GrowthLogs { get; set; }
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Reminder>? Reminders { get; set; }
    public ICollection<ReminderHistory>? ReminderHistory { get; set; }
}
