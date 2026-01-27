using PlantApp.Domain.Dtos.Analytics;

namespace PlantApp.Domain.Dtos.ML;

public class PlantPredictionDto
{
    public required string PlaceName { get; set; }
    public required string PlantName { get; set; }
    public required PlantMLInput MLInput { get; set; }
}
