using PlantApp.Data.Models;

namespace PlantApp.Domain.Interfaces.Data;

public interface IImageService
{
    public Task AddImagesToEntityAsync(IHasImages entity, List<string> urls);
    public Task AddImagesSafeAsync(IHasImages entity, List<string> urls);
    public Task RemoveImageFromEntityAsync(IHasImages entity, int imageId);
}
