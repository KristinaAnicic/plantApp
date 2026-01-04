using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Image : BaseEntity
{
    public required string Url { get; set; }
    public string? Copyright { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public int? UserId { get; set; }

    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    public ICollection<GrowthLog> GrowthLogs { get; set; } = new List<GrowthLog>();
    public ICollection<PlantExchange> PlantExchanges { get; set; } = new List<PlantExchange>();
    public ICollection<Planted> Planted { get; set; } = new List<Planted>();
}
