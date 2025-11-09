using PlantApp.Data.Models;
using System.Text.Json.Serialization;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public ExchangeType? ExchangeType { get; set; }
    public string? Place { get; set; }
    public required string Image { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? Price { get; set; }
}
