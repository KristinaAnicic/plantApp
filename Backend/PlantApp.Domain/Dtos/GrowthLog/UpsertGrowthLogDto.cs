using System.ComponentModel.DataAnnotations;

namespace PlantApp.Domain.Dtos.GrowthLog;

public class UpsertGrowthLogDto
{
    public int Id { get; set; }
    public int? PlantedId { get; set; }
    public int? PlantGroupId { get; set; }
    public required string Title { get; set; }
    public string? Note { get; set; }
    [Required]
    public int PlantStatusId { get; set; }
    public List<string>? Images { get; set; }
    public DateOnly ObservationDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}
