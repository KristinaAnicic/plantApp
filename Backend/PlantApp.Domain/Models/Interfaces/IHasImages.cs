namespace PlantApp.Domain.Models.Interfaces;

public interface IHasImages
{
    ICollection<Image> Images { get; set; }
}
