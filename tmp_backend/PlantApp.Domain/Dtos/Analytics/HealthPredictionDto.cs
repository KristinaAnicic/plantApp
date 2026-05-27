namespace PlantApp.Domain.Dtos.Analytics;

public class HealthPredictionDto
{
    public string PlantName { get; set; } = string.Empty;
    public string PlaceName { get; set; } = string.Empty;
    public List<float> MonthlyPrediction { get; set; } = new();
    public float CurrentSuccessProbability { get; set; }
}
