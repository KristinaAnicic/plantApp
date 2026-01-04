using PlantApp.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class GrowthLog : BaseEntity, IHasImages
{
    [ForeignKey(nameof(PlantedId))]
    public Planted? Planted { get; set; }
    public required int PlantedId { get; set; }

    public string? Note { get; set; }

    [ForeignKey(nameof(PlantStatusId))]
    public PlantStatus? PlantStatus { get; set; }
    public int PlantStatusId { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();
}
