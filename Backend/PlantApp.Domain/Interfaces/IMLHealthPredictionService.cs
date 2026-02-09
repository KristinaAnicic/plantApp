using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Interfaces;

public interface IMLHealthPredictionService
{
    public Task TrainModelAsync();
    public Task<float> PredictHealthScore(HealthPredictionMLInput input);
    public Task<List<float>> PredictHealthScoresBatch(List<HealthPredictionMLInput> inputs);
}
