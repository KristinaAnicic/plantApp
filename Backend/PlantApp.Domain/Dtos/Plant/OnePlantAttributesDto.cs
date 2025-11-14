namespace PlantApp.Domain.Dtos.Plant;

public class OnePlantAttributesDto
{
    public List<ReferenceDto> SpreadTypes { get; set; } = new();
    public List<ReferenceDto> HeightTypes { get; set; } = new();
    public List<ReferenceDto> TimeToFullHeights { get; set; } = new();
    public List<ReferenceDto> HardinessLevels { get; set; } = new();
    public List<ReferenceDto> Fragrances { get; set; } = new();
    public List<ReferenceDto> Families { get; set; } = new();
}
