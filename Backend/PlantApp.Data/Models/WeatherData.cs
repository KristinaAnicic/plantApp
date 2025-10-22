using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlantApp.Data.Models;

public class WeatherData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public required DateTime DateTime { get; set; }

    [Required]
    public int CityId { get; set; }

    [ForeignKey(nameof(CityId))]
    public City? City { get; set; }

    public double? Temperature { get; set; }
    [ForeignKey(nameof(WeatherTypeId))]
    public WeatherType? WeatherType { get; set; }
    public int WeatherTypeId { get; set; }
}
