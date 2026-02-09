namespace PlantApp.Domain.Dtos.ML;

public class PlantHealthPredictionDto
{
    public required string PlaceName { get; set; }
    public required string PlantName { get; set; }
    public required HealthPredictionMLInput MLInput { get; set; }
}
