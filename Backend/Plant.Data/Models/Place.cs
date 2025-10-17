using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class Place : BaseEntity
{
    public required string Name { get; set; }
    public string? Address { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public required int UserId { get; set; }
    [ForeignKey(nameof(CityId))]
    public City? City { get; set; }
    public int CityId { get; set; }
}
