using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class UserRating : BaseEntity
{
    [ForeignKey(nameof(RaterId))]
    public User? Rater { get; set; }
    public int RaterId { get; set; }

    [ForeignKey(nameof(RatedId))]
    public User? Rated { get; set; }
    public int RatedId { get; set; }

    public int Rating {  get; set; }
    public required string Comment { get; set; }
}
