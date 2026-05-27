using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Models.Interfaces;

public interface IImageService
{
    public Task AddImagesToEntityAsync(IHasImages entity, List<string> urls);
    public Task AddImagesSafeAsync(IHasImages entity, List<string> urls);
    public Task RemoveImageFromEntityAsync<T>(T entity, int imageId, IRepository<T> entityRepository) where T : class, IHasImages;
    public Task RemoveUnusedImagesAsync();
}
