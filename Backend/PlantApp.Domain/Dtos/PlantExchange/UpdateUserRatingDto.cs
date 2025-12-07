using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.PlantExchange;

public class UpdateUserRatingDto
{
    [Required]
    public int Rating { get; set; }
    [Required]
    public required string Comment { get; set; }
}
