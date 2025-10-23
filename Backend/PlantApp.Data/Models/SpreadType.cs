namespace PlantApp.Data.Models;

public class SpreadType : BaseEntity
{
    public required string Type { get; set; }
    public required decimal MinSpread { get; set; }
    public decimal? MaxSpread { get; set; }
    public string Unit { get; set; } = "m";
    public ICollection<Plant>? Plants { get; set; }
}
