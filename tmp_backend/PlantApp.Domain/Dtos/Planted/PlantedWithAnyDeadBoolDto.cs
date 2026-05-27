namespace PlantApp.Domain.Dtos.Planted;

public class PlantedWithAnyDeadBoolDto
{
    public List<PlantedDto> Planted { get; set; } = new List<PlantedDto>();
    public int NumOfDeadPlants { get; set; }
}
