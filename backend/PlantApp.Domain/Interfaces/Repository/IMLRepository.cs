using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IMLRepository
{
    public Task<List<PlantedGrowthLogOverviewDto>> GetHealthPredictionTrainingData();
    public Task<List<HealthPredictionRecord>> GetUserHealthPredictionInputData(int userId, int? plantedId);
    public Task<List<RecommendationMLInput>> GetRecommendationMLInput();
    public Task<List<RecommendationMLInput>> GetUserRecommendationInputData(int userId);
}
