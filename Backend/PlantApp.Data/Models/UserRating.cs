using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class UserRating : BaseEntity
{
    [ForeignKey(nameof(PlantExchangeId))]
    public PlantExchange? PlantExchange { get; set; }
    public int PlantExchangeId { get; set; }

    [ForeignKey(nameof(RaterId))]
    public User? Rater { get; set; }
    public int RaterId { get; set; }

    [ForeignKey(nameof(RatedId))]
    public User? Rated { get; set; }
    public int RatedId { get; set; }

    public int Rating {  get; set; }
    public required string Comment { get; set; }
}
