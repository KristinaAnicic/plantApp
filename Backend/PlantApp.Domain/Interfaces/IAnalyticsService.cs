using PlantApp.Domain.Dtos.Analytics;

namespace PlantApp.Domain.Interfaces;

public interface IAnalyticsService
{
    public Task<AnalyticsDto> GetAnalytics();
    public Task<PlantedAnalyticsDto> GetPlantedAnalytics(int plantedId);
    public Task<PlantGroupAnalytics> GetPlantedGroupAnalytics(int plantGroupId, int? year = null);
}
