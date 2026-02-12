namespace PlantApp.Domain.Dtos.Analytics;

public class PlantGrowthHeight
{
    public int Month { get; set; }
    public decimal Height { get; set; }
    public List<string> ActiveAttributes { get; set; } = new();
}
