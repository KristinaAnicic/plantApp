using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class UpsertPlantExchangeDto
{
    public int? PlantedId { get; set; }
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Content { get; set; }
    [Required]
    public required string PlantStatus { get; set; }
    [Required]
    public required string Contact { get; set; }
    [Required]
    public required string MainImage { get; set; }
    public bool IsActive { get; set; }
    [Required]
    public required int ExchangeTypeId { get; set; }
    [Required]
    public required string City { get; set; }
    [Required]
    public required int CountryId { get; set; }
    public string? ExchangeFor { get; set; }
    public decimal? Price { get; set; }
    [Required]
    public required string Shipping { get; set; }

    public List<string>? Images { get; set; }
}
