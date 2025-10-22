namespace PlantApp.Data.Models;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<User>? Users { get; set; }
}
