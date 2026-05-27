namespace PlantApp.Domain.Dtos.PlantNet;

public class PredictedOrgan
{
    public string? Image { get; set; }
    public string? Filename { get; set; }
    public string? Organ { get; set; }
    public double? Score { get; set; }
}