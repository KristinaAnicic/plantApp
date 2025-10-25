namespace PlantApp.Data.Models;

public class Fragrance : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<Plant>? Plants { get; set; }
}
