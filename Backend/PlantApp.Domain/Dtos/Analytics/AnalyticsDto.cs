using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.Analytics;

public class AnalyticsDto
{
    public required PlantSummary Summary { get; set; }
    public List<ReminderStat> ReminderStats { get; set; } = new();
    public List<HealthOverview> HealthStats { get; set; } = new();
    public List<GrowthLogActivity> GrowthLogActivity { get; set; } = new();
    public List<ActionFrequencyDto> ActionStats { get; set; } = new();
    public PlantHallOfFame? HallOfFame { get; set; }

}

public class PlantSummary
{
    public int NumOfPlants { get; set; }
    public int NumOfCurrentPlants { get; set; }
    public int NumOfDeadPlants => NumOfPlants - NumOfCurrentPlants;
    public int NumOfLogsThisYear { get; set; }
    public int NumOfLogsOverAll { get; set; }
    public DateOnly? FirstPlantedDate { get; set; }
}

public class ReminderStat
{
    public required string Label { get; set; } // on time, delayed, missed
    public int Percentage {  get; set; }
}

public class HealthOverview
{
    public required string Label { get; set; } // healthy, stressed, dormant
    public int Percentage { get; set; }
}

public class GrowthLogActivity
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Count { get; set; }
}

public class ActionFrequencyDto
{
    public required string ActionType { get; set; } // Watering, Fertilizing, Misting
    public int Count { get; set; }
}

public class PlantHallOfFame
{
    public PlantedDto? OldestPlant { get; set; } // Plant that is the longest alive
    public int DaysAlive { get; set; }
    public PlantedDto? MostResilientPlant { get; set; } // Plant with the most "Missed" reminders that is alive
    public int NumOfLateReminder { get; set; }
}
