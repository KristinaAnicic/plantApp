using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.Analytics;

public class PlantHallOfFame
{
    public PlantedDto? OldestPlant { get; set; } // Plant that is the longest alive
    public int DaysAlive { get; set; }
    public PlantedDto? MostResilientPlant { get; set; } // Plant with the most "Missed" reminders that is alive
    public int NumOfLateReminder { get; set; }
}
