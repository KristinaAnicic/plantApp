namespace PlantApp.Domain.Dtos.PlantNet;

public class PlantNetQuery
{
    public string? Project { get; set; }
    public List<string>? Images { get; set; }
    public List<string>? Organs { get; set; }
    public bool? IncludeRelatedImages { get; set; }
    public bool? NoReject { get; set; }
    public string? Type { get; set; }
}
