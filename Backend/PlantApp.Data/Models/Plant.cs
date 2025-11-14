using PlantApp.Data.Models.Categories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Plant : BaseEntity
{
    [Required]
    public required string BotanicalName { get; set; }
    public required string CommonName { get; set; }
    //public bool isSynonim {  get; set; }
    [ForeignKey(nameof(SynonymParentPlantId))]
    public Plant? SynonymParentPlant {  get; set; }
    public int? SynonymParentPlantId { get; set; }

    [ForeignKey(nameof(FragranceId))]
    public Fragrance? Fragrance { get; set; }
    public int? FragranceId { get; set; }

    [ForeignKey(nameof(HardinessLevelId))]
    public HardinessLevel? HardinessLevel { get; set; }
    public int? HardinessLevelId { get; set; }
    public bool? IsSpecie {  get; set; }
    public bool? IsGenus { get; set; }
    public bool? IsPlantForPollinators { get; set; }
    public bool? IsLowMaintenance { get; set; }
    public bool? IsDroughtResistant { get; set; }

    [ForeignKey(nameof(SpreadTypeId))]
    public SpreadType? SpreadType { get; set; }
    public int? SpreadTypeId { get; set; }

    [ForeignKey(nameof(HeightTypeId))]
    public HeightType? HeightType { get; set; }
    public int? HeightTypeId { get; set; }

    [ForeignKey(nameof(TimeToFullHeightId))]
    public TimeToFullHeight? TimeToFullHeight { get; set; }
    public int TimeToFullHeightId { get; set; }
    public string? Toxicity {  get; set; }
    public string? Cultivation {  get; set; }
    public string? PestResistance { get; set; }
    public string? DiseaseResistance { get; set; }
    public string? Pruning {  get; set; }
    public string? Propagation { get; set; }

    [ForeignKey(nameof(FamilyId))]
    public PlantFamily? Family { get; set; }
    public int? FamilyId { get; set; }
    public string? EntityDescription { get; set; }
    public string? GenusDescription { get; set; }

    public ICollection<SoilType> SoilTypes { get; set; } = new List<SoilType>();
    public ICollection<Plant> Synonyms { get; set; } = new List<Plant>();
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Sunlight> Sunlights { get; set; } = new List<Sunlight>();
    public ICollection<Aspect> Aspects { get; set; } = new List<Aspect>();
    public ICollection<Moisture> Moistures { get; set; } = new List<Moisture>();
    public ICollection<Ph> Phs { get; set; } = new List<Ph>();
    public ICollection<Exposure> Exposures { get; set; } = new List<Exposure>();
    public ICollection<Habit> Habits { get; set; } = new List<Habit>();
    public ICollection<Season> Seasons { get; set; } = new List<Season>();
    public ICollection<Planted> PlantedList { get; set; } = new List<Planted>();
}
