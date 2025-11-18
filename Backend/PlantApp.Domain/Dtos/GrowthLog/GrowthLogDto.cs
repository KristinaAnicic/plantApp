namespace PlantApp.Domain.Dtos.GrowthLog;

public class GrowthLogDto
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public string? PlantStatus { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<ImageDto>? Images { get; set; }
}
