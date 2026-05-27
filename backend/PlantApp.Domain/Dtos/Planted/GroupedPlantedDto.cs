using PlantApp.Domain.Dtos.PlantPlace;

namespace PlantApp.Domain.Dtos.Planted;

public class GroupedPlantedDto
{
    public PlaceDto? Place { get; set; }
    public List<PlantedDto>? Planted { get; set; }
}
