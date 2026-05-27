namespace PlantApp.Domain.Dtos.PlantGroup;

public class UpsertPlantGroupDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
