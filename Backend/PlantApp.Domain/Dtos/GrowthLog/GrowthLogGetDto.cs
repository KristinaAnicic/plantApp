using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Dtos.GrowthLog;

public class GrowthLogGetDto
{
    public int Id { get; set; }
    public int PlantedId { get; set; }
    public string? Plant { get; set; }
    public required string Title { get; set; }
    public string? Note { get; set; }
    public ReferenceDto? PlantStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<ImageDto>? Images { get; set; }
}
