namespace Plant.Data.Models;

public class Image : BaseEntity
{
    public required string Name { get; set; }
    public string? Copyright { get; set; }
    public ICollection<Plant>? Plants { get; set; }
    public ICollection<GrowthLog>? GrowthLogs { get; set; }
}
