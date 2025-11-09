using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class HardinessLevel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Level { get; set; }
    public required string Description { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
