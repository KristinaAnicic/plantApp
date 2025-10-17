namespace Plant.Data.Models;

public class SpreadType : BaseEntity
{
    public required string Type { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
