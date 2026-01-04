namespace PlantApp.Data.Models.Interfaces;

public interface IHasImages
{
    ICollection<Image> Images { get; set; }
}
