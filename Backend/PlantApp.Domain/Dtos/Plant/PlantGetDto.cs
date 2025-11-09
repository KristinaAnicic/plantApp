namespace PlantApp.Domain.Dtos.Plant;

public class PlantGetDto : PlantDto
{
    public string? Fragrance {  get; set; }
    public string? HardinessLevel { get; set; }
    public bool? IsSpecie { get; set; }
    public bool? IsGenus { get; set; }
    public bool? IsPlantForPollinators { get; set; }
    public bool? IsLowMaintenance { get; set; }
    public bool? IsDroughtResistant { get; set; }
    public string? SpreadType { get; set; }
    public string? HeightType { get; set; }
    public string? TimeToFullHeight { get; set; }
    public string? Toxicity { get; set; }
    public string? Cultivation { get; set; }
    public string? PestResistance { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? Pruning { get; set; }
    public string? Propagation { get; set; }
    public string? Family { get; set; }
    public string? GenusDescription { get; set; }
    public string? SoilTypes { get; set; }
    public List<ImageDto>? Images { get; set; }
    public string? Sunlights { get; set; }
    public string? Aspects { get; set; }
    public string? Moistures { get; set; }
    public string? Phs { get; set; }
    public string? Exposures { get; set; }
    public List<string>? Habits { get; set; }
    public List<string>? Seasons { get; set; }
}
