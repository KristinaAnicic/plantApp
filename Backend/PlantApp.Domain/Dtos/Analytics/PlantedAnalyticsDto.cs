using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantedAnalyticsDto
{
    public List<float> MonthlyHealthPrediction { get; set; } = new();
    public List<PlantGrowthHeight> PlantGrowthHeight { get; set; } = new();

}
