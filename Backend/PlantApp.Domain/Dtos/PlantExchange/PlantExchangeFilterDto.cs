namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeFilterDto
{
    public string? Name { get; set; }
    public int? ExchangeType { get; set; }
    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo {  get; set; }
    public string? City { get; set; }
}
