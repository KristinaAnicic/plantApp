namespace Plant.Data.Models;

public class Fragnance : BaseEntity
{
    public required string Name { get; set; }

    public ICollection<Plant>? Plants { get; set; }
}
