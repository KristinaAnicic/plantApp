using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plant.Data.Models;

public class WeatherType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string weatherType { get; set; }
    public ICollection<WeatherData>? WeatherDatas { get; set; }
}
