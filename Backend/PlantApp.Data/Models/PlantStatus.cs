namespace PlantApp.Data.Models;

public class PlantStatus : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Planted>? PlantedList { get; set; }
    public ICollection<GrowthLog>? GrowthLogList { get; set; }
}
