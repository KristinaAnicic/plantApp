using PlantApp.Domain.Dtos.Analytics;

namespace PlantApp.Domain.Interfaces;

public interface IAnalyticsService
{
    public Task<AnalyticsDto> GetAnalytics();
}
