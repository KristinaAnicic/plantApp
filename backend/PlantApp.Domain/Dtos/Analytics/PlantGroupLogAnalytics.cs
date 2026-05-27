using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantGroupLogAnalytics
{
    public int Month { get; set; }
    public float AvgHealth { get; set; }
}
