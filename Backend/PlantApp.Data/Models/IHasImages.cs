namespace PlantApp.Data.Models;

public interface IHasImages
{
    ICollection<Image> Images { get; set; }
}
