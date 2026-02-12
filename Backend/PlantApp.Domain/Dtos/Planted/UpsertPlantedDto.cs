namespace PlantApp.Domain.Dtos.Planted;

public class UpsertPlantedDto
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int PlantId { get; set; }
    public int? PlantGroupId { get; set; }
    public required int PlaceId { get; set; }
    public DateOnly? DatePlanted { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string? Source { get; set; }
    public string? Note { get; set; }
    public bool IsOutside { get; set; } = false;
    public string? Image { get; set; }
    public int PlantStatusId { get; set; }
    public List<string>? Images { get; set; } = new List<string>();
}
