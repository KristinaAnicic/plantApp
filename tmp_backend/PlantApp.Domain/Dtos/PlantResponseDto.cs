namespace PlantApp.Domain.Dtos;

public class PlantResponseDto
{
    public int Id { get; set; }
    public bool? IsSynonym { get; set; }
    public int? SynonymParentPlantId { get; set; }
    public string? BotanicalName { get; set; }
    public bool? NotedForFragrance { get; set; }
    public int? FragranceId { get; set; }
    public string? CommonName { get; set; }
    public int? HardinessLevel { get; set; }
    public bool? IsGenus { get; set; }
    public bool? IsSpecie { get; set; }
    public bool? IsPlantsForPollinators { get; set; }
    public bool? IsLowMaintenance { get; set; }
    public bool? IsDroughtResistance { get; set; }
    public int? SpreadTypeId { get; set; }
    public int? HeightTypeId { get; set; }
    public int? TimeToFullHeightId { get; set; }
    public int? FoliageId { get; set; }
    public string? Toxicity { get; set; }
    public string? Cultivation { get; set; }
    public string? PestResistance { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? Pruning { get; set; }
    public string? Propagation { get; set; }
    public int? FamilyId { get; set; }
    public string? EntityDescription { get; set; }
    public string? GenusDescription { get; set; }

    public string ToCsvLine()
    {
        return $"{Id},\"{IsSynonym}\",\"{SynonymParentPlantId}\",\"{BotanicalName}\",\"{NotedForFragrance}\",\"{FragranceId}\",\"{CommonName}\",\"{HardinessLevel}\",\"{IsGenus}\",\"{IsSpecie}\",\"{IsPlantsForPollinators}\",\"{IsLowMaintenance}\",\"{IsDroughtResistance}\",\"{SpreadTypeId}\",\"{HeightTypeId}\",\"{TimeToFullHeightId}\",\"{FoliageId}\",\"{Cultivation}\",\"{PestResistance}\",\"{DiseaseResistance}\",\"{Pruning}\",\"{Propagation}\",\"{FamilyId}\",\"{EntityDescription}\",\"{GenusDescription}\"";
    }
}
