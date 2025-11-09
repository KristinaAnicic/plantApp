using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class HeightType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public required decimal MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
    public string Unit { get; set; } = "m";
    public ICollection<Plant>? Plants { get; set; }
}
