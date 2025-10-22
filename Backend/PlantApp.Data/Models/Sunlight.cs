namespace PlantApp.Data.Models;

public class Sunlight : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
