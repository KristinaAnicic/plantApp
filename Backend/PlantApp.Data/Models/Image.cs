using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Image : BaseEntity
{
    public required string Url { get; set; }
    public string? Copyright { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public int? UserId { get; set; }

    public ICollection<Plant>? Plants { get; set; }
    public ICollection<GrowthLog>? GrowthLogs { get; set; }
    public ICollection<PlantExchange>? PlantExchanges { get; set; }
    public ICollection<Planted>? Planted { get; set; }
}
