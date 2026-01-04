using PlantApp.Data.Models;
using PlantApp.Data.Models.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Services.Data;

public class ImageService(
    IRepository<Image> imageRepository
) : IImageService
{
    public int currentUser = 3;
    public async Task AddImagesToEntityAsync(IHasImages entity, List<string> urls)
    {
        var distinctUrls = urls.Select(u => u.Trim()).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();

        var existingImages = await imageRepository.GetAllByKeyAsync(img => distinctUrls.Contains(img.Url));

        foreach (var url in distinctUrls)
        {
            var image = existingImages.FirstOrDefault(i => i.Url == url);

            if (image != null)
            {
                if (image.UserId != null && image.UserId != currentUser)
                    throw new InvalidOperationException($"Cannot add an image that belongs to another user: {url}");

                if (!entity.Images.Any(i => i.Id == image.Id))
                    entity.Images.Add(image);
            }
            else
            {
                entity.Images.Add(new Image
                {
                    Url = url,
                    UserId = currentUser
                });
            }
        }
    }

    public async Task AddImagesSafeAsync(IHasImages entity, List<string> urls)
    {
        var distinctUrls = urls.Select(u => u.Trim()).Where(u => !string.IsNullOrWhiteSpace(u)).Distinct().ToList();

        var existingImages = await imageRepository.GetAllByKeyAsync(img => distinctUrls.Contains(img.Url));

        foreach (var url in distinctUrls)
        {
            var image = existingImages.FirstOrDefault(i => i.Url == url);

            if (image != null)
            {
                if (image.UserId != null && image.UserId != currentUser)
                    continue;

                if (!entity.Images.Any(i => i.Id == image.Id))
                    entity.Images.Add(image);
            }
            else
            {
                entity.Images.Add(new Image
                {
                    Url = url,
                    UserId = currentUser
                });
            }
        }
    }


    public async Task<string?> RemoveImageFromEntityAsync<T>(T entity, int imageId, IRepository<T> entityRepository) where T : class, IHasImages
    {
        var image = await imageRepository.GetByIdAsync(imageId);
        if (image == null)
            throw new ArgumentException("Image not found");

        var deletedUrl = image.Url;
        entity.Images.Remove(image);

        await entityRepository.UpdateAsync(entity);
        if (!image.Plants.Any() && !image.GrowthLogs.Any() && !image.Planted.Any() && !image.PlantExchanges.Any())
        {
            await imageRepository.DeleteAsync(image, false);
            return deletedUrl;
        }

        return null;
    }

}
