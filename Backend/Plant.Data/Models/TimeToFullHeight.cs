namespace Plant.Data.Models;

public class TimeToFullHeight : BaseEntity
{
    public required string Time {  get; set; }
    public ICollection<Plant>? Plants { get; set; }
}
