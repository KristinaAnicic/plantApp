using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class TimeToFullHeight
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name {  get; set; }
    public required int MinTime { get; set; }
    public int? MaxTime { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
