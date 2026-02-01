using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? Contact { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
    public int RoleId { get; set; }
    public char Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }
    //public decimal Rating { get; set; }

    public ICollection<Place> Places { get; set; } = new List<Place>();
    public ICollection<PlantExchange> PlantExchanges { get; set; } = new List<PlantExchange>();
    public ICollection<UserRating> RatingsGiven { get; set; } = new List<UserRating>();
    public ICollection<UserRating> RatingsReceived { get; set; } = new List<UserRating>();  
    public ICollection<Image> UploadedImages { get; set; } = new List<Image>();
}
