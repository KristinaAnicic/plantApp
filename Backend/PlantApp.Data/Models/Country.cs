using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Country
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Iso { get; set; }
    public required string Iso3 { get; set; }
    public required string Name { get; set; }   
    public required int PhoneCode { get; set; }
    public ICollection<City>? Cities { get; set; }
}
