namespace PlantApp.Data.Models;

public class ExchangeType : BaseEntity
{
    public required string Type { get; set; }

    public ICollection<PlantExchange>? PlantExchanges { get; set; } 
}