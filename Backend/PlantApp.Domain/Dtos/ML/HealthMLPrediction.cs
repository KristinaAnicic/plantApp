using Microsoft.ML.Data;

namespace PlantApp.Domain.Dtos.ML;

public class HealthMLPrediction
{
    [ColumnName("Score")]
    public float PredictedHealthScore { get; set; }
}
