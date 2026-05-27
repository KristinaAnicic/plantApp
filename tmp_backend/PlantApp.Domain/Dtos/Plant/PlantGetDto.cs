namespace PlantApp.Domain.Dtos.Plant;

public class PlantGetDto : PlantDto
{
    public ReferenceDto? Fragrance {  get; set; }
    public ReferenceDto? HardinessLevel { get; set; }
    public bool? IsSpecie { get; set; }
    public bool? IsGenus { get; set; }
    public bool? IsPlantForPollinators { get; set; }
    public bool? IsLowMaintenance { get; set; }
    public bool? IsDroughtResistant { get; set; }
    public ReferenceDto? SpreadType { get; set; }
    public ReferenceDto? HeightType { get; set; }
    public ReferenceDto? TimeToFullHeight { get; set; }
    public string? Toxicity { get; set; }
    public string? Cultivation { get; set; }
    public string? PestResistance { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? Pruning { get; set; }
    public string? Propagation { get; set; }
    public ReferenceDto? Family { get; set; }
    public string? GenusDescription { get; set; }
    public List<ReferenceDto>? SoilTypes { get; set; }
    public List<ImageDto>? Images { get; set; }
    public List<ReferenceDto>? Sunlights { get; set; }
    public List<ReferenceDto>? Aspects { get; set; }
    public List<ReferenceDto>? Moistures { get; set; }
    public List<ReferenceDto>? Phs { get; set; }
    public List<ReferenceDto>? Exposures { get; set; }
    public List<ReferenceDto>? Habits { get; set; }
    public List<ReferenceDto>? Seasons { get; set; }
    public List<ReferenceDto>? Synonyms { get; set; }
    public ReferenceDto? ParentPlant { get; set; }
}
