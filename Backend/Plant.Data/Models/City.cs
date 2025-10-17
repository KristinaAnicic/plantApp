using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string Name { get; set; }

    [ForeignKey(nameof(CountryId))]
    public Country? Country { get; set; }
    public required int CountryId { get; set; }
    public ICollection<Place>? Places { get; set; }
}
