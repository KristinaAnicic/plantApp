using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.PlantPlace;

namespace PlantApp.Domain.Dtos.Planted;

public class GroupedPlantedFamilyPlaceDto
{
    public PlaceDto? Place { get; set; }
    public ReferenceDto? Family { get; set; }
    public List<PlantedDto>? Planted { get; set; }
}
