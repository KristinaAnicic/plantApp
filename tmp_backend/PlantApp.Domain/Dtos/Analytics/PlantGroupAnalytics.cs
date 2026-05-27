using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantGroupAnalytics
{
    public List<PlantGroupLogAnalytics> GroupLogAnalytics { get; set; }
    public List<GroupedGrowthAnalytics> GrowthAnalytics { get; set; }
}
