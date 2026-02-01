using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class SpreadType : IReferenceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required decimal MinSpread { get; set; }
    public decimal? MaxSpread { get; set; }
    public string Unit { get; set; } = "m";
    public ICollection<Plant>? Plants { get; set; }
}
