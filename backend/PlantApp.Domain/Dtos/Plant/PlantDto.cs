using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.Plant;

public class PlantDto
{
    public required int PlantId { get; set; }
    public required string BotanicalName { get; set; }
    public required string CommonName { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? EntityDescription { get; set; }
    public string? Image {  get; set; }
}
