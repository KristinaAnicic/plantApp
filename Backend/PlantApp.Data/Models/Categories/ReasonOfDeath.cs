using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class ReasonOfDeath
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Planted>? Plants { get; set; }
}