using PlantApp.Data.Models.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Place : BaseEntity, IReferenceEntity
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
    [Range(1, 5, ErrorMessage = "Sunlight intensity must be between 1 and 5.")]
    public int SunlightIntensity { get; set; }
    [Range(1, 5, ErrorMessage = "Humidity intensity must be between 1 and 5.")]
    public int HumidityIntensity { get; set; }

    public ICollection<Planted> PlantedList { get; set; } = new List<Planted>();
    public ICollection<GrowthLog> GrowthLogs { get; set; } = new List<GrowthLog>();
}
