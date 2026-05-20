namespace PlantApp.Domain.Dtos.PlantNet;

public class PlantNetResponse
{
    public PlantNetQuery? Query { get; set; }
    public List<PredictedOrgan>? PredictedOrgans { get; set; }
    public string? Language { get; set; }
    public string? PreferedReferential { get; set; }
    public string? BestMatch { get; set; }
    public List<PlantNetResult>? Results { get; set; }
    public string? Version { get; set; }
    public int? RemainingIdentificationRequests { get; set; }
}
