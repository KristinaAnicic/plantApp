using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Place : BaseEntity
{
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Note { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public required int UserId { get; set; }

    [ForeignKey(nameof(CountryId))]
    public Country? Country { get; set; }
    public int CountryId { get; set; }

    public ICollection<Planted>? PlantedList { get; set; }
}
