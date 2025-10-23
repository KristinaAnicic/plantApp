namespace PlantApp.Data.Models;

public class HeightType : BaseEntity
{
    public required string Type { get; set; }
    public required decimal MinHeight { get; set; }
    public decimal? MaxHeight { get; set; }
    public string Unit { get; set; } = "m";
    public ICollection<Plant>? Plants { get; set; }
}
