using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models.Categories;

public class PlantSeasonAttribute
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [ForeignKey(nameof(PlantId))]
    public Plant? Plant { get; set; }
    public int PlantId { get; set; }

    [ForeignKey(nameof(SeasonId))]
    public Season? Season { get; set; }
    public int SeasonId { get; set; }

    [ForeignKey(nameof(PlantAttributeTypeId))]
    public PlantAttributeType? PlantAttributeType { get; set; }
    public int PlantAttributeTypeId { get; set; }

    public string Colour { get; set; } = null!;
}
