using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class PlaceHistory : BaseEntity
{
    [ForeignKey(nameof(PlantedId))]
    public Planted Planted { get; set; } = null!;
    public int PlantedId { get; set; }

    [ForeignKey(nameof(PlaceId))]
    public Place Place { get; set; } = null!;
    public int PlaceId { get; set; }

    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
}
