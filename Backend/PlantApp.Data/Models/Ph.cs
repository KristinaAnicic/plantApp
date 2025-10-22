namespace PlantApp.Data.Models;

public class Ph : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
