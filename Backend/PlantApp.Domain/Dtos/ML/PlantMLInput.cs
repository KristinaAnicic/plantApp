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
    [VectorType(3)]
    public required float[] SunlightRequirements { get; set; } = new float[3];
    [VectorType(3)]
    public required float[] MoistureRequirements { get; set; } = new float[3];
    public bool IsLowMaintenance { get; set; }
    public bool IsDroughtResistant { get; set; }

    [ColumnName("Label")]
    public float HealthScore { get; set; }

    public PlantMLInput Clone()
    {
        return (PlantMLInput)this.MemberwiseClone();
    }
}
