namespace Plant.Data.Models;

public class HardinessLevel : BaseEntity
{
    public required string Level { get; set; }
    public required string Description { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
