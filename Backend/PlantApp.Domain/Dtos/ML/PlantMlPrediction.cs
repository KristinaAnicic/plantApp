using Microsoft.ML.Data;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantMLPrediction
{
    [ColumnName("Score")]
    public float PredictedHealthScore { get; set; }
}
