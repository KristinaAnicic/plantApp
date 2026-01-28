using PlantApp.Data.Models;

namespace PlantApp.Domain.Dtos.ML;

public class PlantAnalyticsRecord
{
    public string? PlantName { get; set; }
    public string? PlaceName { get; set; }
    public float SunlightIntensity {  get; set; }
    public float HumidityIntensity {  get; set; }
    public bool IsOutside {  get; set; }
    public string Family {  get; set; }
    public string Hardiness {  get; set; }
    public int PlantStatusId {  get; set; }
    public List<Sunlight> SunlightList { get; set; } = new();
    public List<Moisture> MoistureList { get; set; } = new();
    public bool LowMaintenace { get; set; }
    public bool DroughtResistant { get; set; }
    public float Month { get; set; }
    public float HealthScore { get; set; }
}
