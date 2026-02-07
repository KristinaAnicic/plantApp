using Microsoft.ML.Data;

namespace PlantApp.Domain.Dtos.ML;

public class PlantMLPrediction
{
    [ColumnName("Score")]
    public float PredictedHealthScore { get; set; }
}
