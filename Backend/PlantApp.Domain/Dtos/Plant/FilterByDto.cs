namespace PlantApp.Domain.Dtos.Plant;

public class FilterByDto
{
    public string? Name { get; set; } = null;
    public bool? IsLowMaintenance { get; set; } = null;
    public bool? IsDroughtResistant { get; set; } = null;
    public List<int>? Habits { get; set; } = null;
    public List<int>? SoilType { get; set; } = null;
    public int? Spread { get; set; } = null;
    public int? Height { get; set; } = null;
    public int? TimeToFullHeight { get; set; } = null;
    public int? Exposure { get; set; } = null;
}
