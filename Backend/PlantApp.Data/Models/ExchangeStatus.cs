namespace PlantApp.Data.Models;

public class ExchangeStatus : BaseEntity
{
    public required string Type { get; set; }

    public ICollection<PlantExchange>? PlantExchanges { get; set; }
}