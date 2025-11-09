using PlantApp.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Dtos.Plant;

public class UpsertPlantDto
{
    public int? Id { get; set; } = null;
    public required string BotanicalName { get; set; }
    public required string CommonName { get; set; }
    public int? SynonymParentPlantId { get; set; }
    public int? FragranceId { get; set; }
    public int? HardinessLevelId { get; set; }
    public bool? IsSpecie { get; set; }
    public bool? IsGenus { get; set; }
    public bool? IsPlantForPollinators { get; set; }
    public bool? IsLowMaintenance { get; set; }
    public bool? IsDroughtResistant { get; set; }
    public int? SpreadTypeId { get; set; }
    public int? HeightTypeId { get; set; }
    public int TimeToFullHeightId { get; set; }
    public string? Toxicity { get; set; }
    public string? Cultivation { get; set; }
    public string? PestResistance { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? Pruning { get; set; }
    public string? Propagation { get; set; }
    public int? FamilyId { get; set; }
    public string? EntityDescription { get; set; }
    public string? GenusDescription { get; set; }

    public List<int> SoilTypes { get; set; } = new List<int>();
    public List<int> Images { get; set; } = new List<int>();
    public List<int> Sunlights { get; set; } = new List<int>();
    public List<int> Aspects { get; set; } = new List<int>();
    public List<int> Moistures { get; set; } = new List<int>();
    public List<int> Phs { get; set; } = new List<int>();
    public List<int> Exposures { get; set; } = new List<int>();
    public List<int> Habits { get; set; } = new List<int>();
    public List<int> Seasons { get; set; } = new List<int>();
}
