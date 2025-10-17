using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class Planted : BaseEntity
{
    [ForeignKey(nameof(PlaceId))]
    public Place? Place { get; set; }
    public required int PlaceId { get; set; }
    [ForeignKey(nameof(PlaceId))]
    public Plant? Plant { get; set; }
    public required int PlantId { get; set; }
    public required DateTime DatePlanted { get; set; }
    public string? Source {  get; set; }
    public string? Notes { get; set; }
    [ForeignKey(nameof(PlantStatusId))]
    public PlantStatus? PlantStatus { get; set; }
    public int PlantStatusId { get; set; }
    public ICollection<GrowthLog>? GrowthLogs { get; set; }
    public ICollection<Image>? Images { get; set; }
    public ICollection<Reminder>? Reminders { get; set; }
}
