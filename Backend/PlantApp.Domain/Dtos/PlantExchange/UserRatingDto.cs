using PlantApp.Domain.Dtos.Plant;
using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class UserRatingGetDto
{
    public int Id { get; set; }
    public required ReferenceDto Rater { get; set; }
    public required ReferenceDto Rated { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
