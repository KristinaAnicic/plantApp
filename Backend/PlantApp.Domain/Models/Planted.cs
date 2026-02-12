using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class Planted : BaseEntity, IHasImages
{
    [ForeignKey(nameof(PlaceId))]
    public Place? Place { get; set; }
    public required int PlaceId { get; set; }

    [ForeignKey(nameof(PlantId))]
    public Plant? Plant { get; set; }
    public required int PlantId { get; set; }

    public string? Name { get; set; }
    public required DateOnly DatePlanted { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Source {  get; set; }
    public string? Note { get; set; }
    public bool IsOutside { get; set; } = false;
    public string? Image { get; set; }

    [ForeignKey(nameof(PlantStatusId))]
    public PlantStatus? PlantStatus { get; set; }
    public int PlantStatusId { get; set; }

    [ForeignKey(nameof(PlantGroupId))]
    public PlantGroup? PlantGroup { get; set; }
    public int? PlantGroupId { get; set; }

    public DateOnly? DateOfDeath { get; set; }

    public ICollection<GrowthLog> GrowthLogs { get; set; } = new List<GrowthLog>();
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Reminder> Reminders { get; set; } = new List<Reminder>();
    public ICollection<ReminderHistory> ReminderHistory { get; set; } = new List<ReminderHistory>();
    public ICollection<PlaceHistory> PlaceHistory { get; set; } = new List<PlaceHistory>();
}
