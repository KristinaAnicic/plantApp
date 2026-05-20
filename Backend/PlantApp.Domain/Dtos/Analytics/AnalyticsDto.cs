namespace PlantApp.Domain.Dtos.Analytics;

public class AnalyticsDto
{
    public required PlantSummary Summary { get; set; }
    public List<string> PlantRecommendations { get; set; } = new();
    public List<PercentageSegment> ReminderStats { get; set; } = new();
    public List<PercentageSegment> HealthStats { get; set; } = new();   
    public List<ActionFrequencyDto> ActionStats { get; set; } = new();
    public List<PercentageSegment> GroupPlantSuccess { get; set; } = new();
    public List<PercentageSegment> FamilyPlantSuccess { get; set; } = new();
    public List<MonthlyActivityDto> SeasonalPlanting { get; set; } = new();
    public List<MonthlyActivityDto> GrowthLogActivity { get; set; } = new();
    public PlantHallOfFame? HallOfFame { get; set; }
    public List<HealthPredictionDto> HealthPrediction { get; set; } = new();
}