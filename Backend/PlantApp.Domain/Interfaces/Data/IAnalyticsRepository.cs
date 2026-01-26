using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Analytics;

namespace PlantApp.Domain.Interfaces.Data;

public interface IAnalyticsRepository
{
    public Task<PlantSummary> GetPlantSummary(int userId);
    public Task<List<ReminderStat>> GetReminderStats(int userId);
    public Task<List<HealthOverview>> GetHealthStats(int userId);
    public Task<List<MonthlyActivityDto>> GetGrowthLogStats(int userId, DateTime startDate);
    public Task<List<ActionFrequencyDto>> GetActionFrequency(int userId);
    public Task<List<MonthlyActivityDto>> GetSeasonalNumOfPlantings(int userId);
    public Task<Planted?> GetOldestPlant(int userId);
    public Task<(int, Planted?)> GetMostResilientPlant(int userId);
}
