namespace PlantApp.Domain.Interfaces;

public interface IMLRecommendationService
{
    public Task TrainModelAsync();
    public DateTime GetModelCreationDate();
    public Task<List<string>> RecommendPlantsByuserIdAsync(int userId);
}
