using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.Planted;

public class PlantedReferences
{
    public List<ReferenceDto> Places { get; set; } = new List<ReferenceDto>();
    public List<ReferenceDto> PlantStatuses { get; set; } = new List<ReferenceDto>();
}
