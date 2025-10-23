namespace PlantApp.Data.Models;

public class TimeToFullHeight : BaseEntity
{
    public required string Time {  get; set; }
    public required int MinTime { get; set; }
    public int? MaxTime { get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
