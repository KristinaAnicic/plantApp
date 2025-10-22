namespace PlantApp.Data.Models;

public class HeightType : BaseEntity
{
    public required string Type { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
