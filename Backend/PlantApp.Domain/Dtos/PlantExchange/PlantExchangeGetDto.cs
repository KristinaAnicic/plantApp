
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeGetDto : PlantExchangeDto
{
    public required ReferenceDto User { get; set; }
    public PlantedDto? Planted { get; set; }
    public required string Content { get; set; }
    public required string PlantStatus { get; set; }

    public string? ExchangeFor { get; set; }
    public required string Shipping { get; set; }

    public ICollection<Image>? Images { get; set; }
    public ICollection<UserRating>? UserRatings { get; set; }
}
