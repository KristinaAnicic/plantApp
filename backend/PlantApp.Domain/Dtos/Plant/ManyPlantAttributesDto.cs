namespace PlantApp.Domain.Dtos.Plant;

public class ManyPlantAttributesDto
{
    public List<ReferenceDto> Sunlights { get; set; } = new();
    public List<ReferenceDto> Phs { get; set; } = new();
    public List<ReferenceDto> Moistures { get; set; } = new();
    public List<ReferenceDto> Aspects { get; set; } = new();
    public List<ReferenceDto> SoilTypes { get; set; } = new();
    public List<ReferenceDto> Exposures { get; set; } = new();
    public List<ReferenceDto> Habits { get; set; } = new();
    public List<ReferenceDto> Seasons { get; set; } = new();
}
