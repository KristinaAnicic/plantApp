using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.DiseasePrediction;

public class DiseasePredictionResponse
{
    [JsonPropertyName("results")]
    public List<DiseasePredictionDto> Results { get; set; } = new List<DiseasePredictionDto>();
    [JsonPropertyName("main_prediction")]
    public string? MainPrediction { get; set; }
}
