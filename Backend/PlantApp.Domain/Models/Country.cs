using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class Country : IReferenceEntity
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
