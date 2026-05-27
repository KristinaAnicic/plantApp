using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class AddUserRatingDto
{
    [Required]
    public required int RatedUserId { get; set; }
    [Required]
    public int Rating { get; set; }
    [Required]
    public required string Comment { get; set; }
}
