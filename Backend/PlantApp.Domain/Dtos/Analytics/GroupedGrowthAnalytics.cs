using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.Analytics;

public class GroupedGrowthAnalytics
{
    public PlantedDto Planted { get; set; }
    public List<PlantGrowthHeight> PlantGrowthHeight { get; set; }
}
