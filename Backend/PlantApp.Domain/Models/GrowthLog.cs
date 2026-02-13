using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class GrowthLog : BaseEntity, IHasImages
{
    public required string Title { get; set; }
    public string? Note { get; set; }

    [ForeignKey(nameof(PlantStatusId))]
    public PlantStatus? PlantStatus { get; set; }
    public int PlantStatusId { get; set; }
    public DateOnly ObservationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [ForeignKey(nameof(PlaceId))]
    public int? PlaceId { get; set; }
    public Place? Place { get; set; }

    [ForeignKey(nameof(PlantGroupId))]
    public PlantGroup? PlantGroup { get; set; }
    public int? PlantGroupId { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Planted> Planted { get; set; } = new List<Planted>();
}
