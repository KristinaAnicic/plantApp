using PlantApp.Data.Models.Interfaces;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Interfaces.Data;

public interface IImageService
{
    public Task AddImagesToEntityAsync(IHasImages entity, List<string> urls);
    public Task AddImagesSafeAsync(IHasImages entity, List<string> urls);
    public Task<string?> RemoveImageFromEntityAsync<T>(T entity, int imageId, IRepository<T> entityRepository) where T : class, IHasImages;
}
