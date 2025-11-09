using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models.Categories;

public class Season
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
