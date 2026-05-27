namespace PlantApp.Domain.Dtos.Analytics;

public class ActionFrequencyDto
{
    public required string ActionType { get; set; } // Watering, Fertilizing, Misting
    public int Count { get; set; }
}
