using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class PlantExchange : BaseEntity
{
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public int UserId { get; set; }

    [ForeignKey(nameof(PlantedId))]
    public Planted? Planted { get; set; }
    public int? PlantedId { get; set; }

    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string PlantStatus { get; set; }
    public required string Contact { get; set; }
    public required string MainImage { get; set; }
    public bool IsActive { get; set; }

    [ForeignKey(nameof(ExchangeTypeId))]
    public ExchangeType? ExchangeType { get; set; }
    public required int ExchangeTypeId { get; set; }

    public required string City { get; set; }

    [ForeignKey(nameof(CountryId))]
    public Country? Country { get; set; }
    public required int CountryId { get; set; }

    public string? ExchangeFor { get; set; }
    public decimal? Price { get; set; }
    public required string Shipping {  get; set; }

   /* [ForeignKey(nameof(ExchangeStatusId))]
    public ExchangeStatus? ExchangeStatus {  get; set; }
    public required int ExchangeStatusId { get; set; }*/

    public ICollection<Image>? Images { get; set; }
}
