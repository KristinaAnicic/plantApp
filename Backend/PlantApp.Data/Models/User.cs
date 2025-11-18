using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string DisplayName { get; set; }
    public string? Contact { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role? Role { get; set; }
    public int RoleId { get; set; }
    public char Gender { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public ICollection<Place>? Places { get; set; }
    public ICollection<PlantExchange>? PlantExchanges { get; set; }
    public ICollection<UserRating>? RatingsGiven { get; set; }
    public ICollection<UserRating>? RatingsReceived { get; set; }
    public ICollection<Image>? UploadedImages { get; set; }
}
