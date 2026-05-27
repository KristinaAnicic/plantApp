namespace PlantApp.Domain.Dtos.PlantGroup;

public class PlantGroupDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int NumOfPlants { get; set; }
}
