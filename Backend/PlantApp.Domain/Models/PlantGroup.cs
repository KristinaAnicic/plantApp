using PlantApp.Domain.Models.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Domain.Models;

public class PlantGroup : BaseEntity, IReferenceEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    public int UserId { get; set; }

    public ICollection<GrowthLog> GrowthLogs { get; set; } = new List<GrowthLog>();
    public ICollection<Planted> PlantedList { get; set; } = new List<Planted>();
}
