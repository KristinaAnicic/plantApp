using PlantApp.Domain.Dtos.Plant;
using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public ReferenceDto? ExchangeType { get; set; }
    public string? Place { get; set; }
    public required string Image { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; set; }
    public DateTime CreatedAt { get; set; }
}
