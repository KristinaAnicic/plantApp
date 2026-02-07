using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Interfaces;

public interface IMLService
{
    public Task TrainModelAsync();
    public Task<float> PredictHealthScore(PlantMLInput input);
    public Task<List<float>> PredictHealthScoresBatch(List<PlantMLInput> inputs);
}
