namespace PlantApp.Data.Models;

public class Moisture : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
