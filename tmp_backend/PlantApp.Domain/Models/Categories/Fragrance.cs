using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class Fragrance : IReferenceEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }

    public ICollection<Plant>? Plants { get; set; }
}
