using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IMLRepository
{
    public Task<List<HealthPredictionRecord>> GetHealthPredictionTrainingData();
    public Task<List<HealthPredictionRecord>> GetUserHealthPredictionInputData(int userId);
    public Task<List<RecommendationMLInput>> GetRecommendationMLInput();
    public Task<List<RecommendationMLInput>> GetUserRecommendationInputData(int userId);
}
