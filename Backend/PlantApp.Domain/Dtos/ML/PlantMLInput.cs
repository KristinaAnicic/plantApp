using Microsoft.ML.Data;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantMLInput
{
    public float SunlightIntensity { get; set; }
    public float HumidityIntensity { get; set; }
    public bool IsOutside { get; set; }
    public float Month { get; set; }

    public required string PlantFamily { get; set; }
    public required string HardinessLevel { get; set; }
    public required string SunlightRequirements { get; set; }
    public required string MoistureRequirements { get; set; }
    public bool IsLowMaintenance { get; set; }
    public bool IsDroughtResistant { get; set; }

    [ColumnName("Label")]
    public float HealthScore { get; set; }

    public PlantMLInput Clone()
    {
        return (PlantMLInput)this.MemberwiseClone();
    }
}
