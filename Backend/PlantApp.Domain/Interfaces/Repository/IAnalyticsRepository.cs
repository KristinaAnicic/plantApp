using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.Analytics;
using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IAnalyticsRepository
{
    public Task<PlantSummary> GetPlantSummary(int userId);
    public Task<List<PercentageSegment>> GetReminderStats(int userId);
    public Task<List<PercentageSegment>> GetHealthStats(int userId);
    public Task<List<MonthlyActivityDto>> GetGrowthLogStats(int userId, DateTime startDate);
    public Task<List<ActionFrequencyDto>> GetActionFrequency(int userId);
    public Task<List<MonthlyActivityDto>> GetSeasonalNumOfPlantings(int userId);
    public Task<Planted?> GetOldestPlant(int userId);
    public Task<(int, Planted?)> GetMostResilientPlant(int userId);
}
