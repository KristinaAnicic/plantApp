using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class PlantExchangeGetDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Contact { get; set; }
    public ReferenceDto? ExchangeType { get; set; }
    public ReferenceDto? Country { get; set; }
    public required string City { get; set; }
    public required string Image { get; set; }
    public decimal? Price { get; set; }
    public DateTime CreatedAt { get; set; }
    public required ReferenceDto User { get; set; }
    public PlantedDto? Planted { get; set; }
    public required string PlantStatus { get; set; }
    public string? ExchangeFor { get; set; }
    public required string Shipping { get; set; }
    public double? UserRating { get; set; }

    public List<ImageDto>? Images { get; set; }
    public List<UserRatingGetDto>? UserRatings { get; set; }
}
