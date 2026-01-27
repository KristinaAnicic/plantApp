namespace PlantApp.Domain.Dtos.Analytics;

public class PlantSummary
{
    public int NumOfPlants { get; set; }
    public int NumOfCurrentPlants { get; set; }
    public int NumOfDeadPlants => NumOfPlants - NumOfCurrentPlants;
    public int NumOfLogsThisYear { get; set; }
    public int NumOfLogsOverAll { get; set; }
    public DateOnly? FirstPlantedDate { get; set; }
}
