using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeReferences
{
    public List<ReferenceDto>? Planted {  get; set; }
    public List<ReferenceDto>? ExchangeTypes {  get; set; }
}
