using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class Plant : BaseEntity
{
    [Required]
    public required string BotanicalName { get; set; }
    public required string CommonName { get; set; }
    //public bool isSynonim {  get; set; }
    public int? SynonimParentPlantId { get; set; }

    [ForeignKey(nameof(FragnanceId))]
    public Fragnance? Fragnance { get; set; }
    public int FragnanceId { get; set; }

    [ForeignKey(nameof(HardinessLevelId))]
    public HardinessLevel? HardinessLevel { get; set; }
    public int HardinessLevelId { get; set; }
    public bool IsAgm {  get; set; }
    public bool IsGenus { get; set; }
    public bool IsPlantsForPollinators { get; set; }
    public bool IsLowMaintenance { get; set; }
    public bool IsDroughtResistance { get; set; }

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
    public string? Propagnation { get; set; }

    [ForeignKey(nameof(FamilyId))]
    public PlantFamily? Family { get; set; }
    public int FamilyId { get; set; }
    public string? EntityDescription { get; set; }
    public string? GenusDescription { get; set; }

    public ICollection<SoilType>? SoilTypes { get; set; }
    public ICollection<Image>? Images { get; set; }
    public ICollection<Sunlight>? Sunlights { get; set; }
    public ICollection<Aspect>? Aspects { get; set; }
    public ICollection<Moisture>? Moistures { get; set; }
    public ICollection<Ph>? Phs { get; set; }
    public ICollection<Exposure>? Exposures { get; set; }
    public ICollection<Habit>? Habits { get; set; }
    public ICollection<Planted>? PlantedList { get; set; }
}
