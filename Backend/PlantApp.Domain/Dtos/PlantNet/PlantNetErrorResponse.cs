namespace PlantApp.Domain.Dtos.PlantNet;

public class PlantNetErrorResponse
{
    public int StatusCode { get; set; }
    public string? Error { get; set; }
    public string? Message { get; set; }
}
