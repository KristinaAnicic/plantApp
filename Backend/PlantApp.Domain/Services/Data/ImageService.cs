using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Data.Models.Interfaces;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Services.Data;

public class ImageService(
    IRepository<Image> imageRepository,
    ICurrentUserContext userContext,
    ILogger<ImageService> logger
) : IImageService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    public async Task AddImagesToEntityAsync(IHasImages entity, List<string> urls)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (urls == null || urls.Count == 0)
        {
            logger.LogInformation("No image URLs provided for entity {EntityType}", entity.GetType().Name);
            return;
        }

        var distinctUrls = urls
            .Select(u => u.Trim())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct()
            .ToList();

        var existingImages = await imageRepository.GetAllByKeyAsync(img => distinctUrls.Contains(img.Url));

        foreach (var url in distinctUrls)
        {
            var image = existingImages.FirstOrDefault(i => i.Url == url);

            if (image != null)
            {
                if (image.UserId != null && image.UserId != CurrentUserId)
                {
                    logger.LogWarning(
                        "User {UserId} attempted to add image {Url} owned by user {OwnerId}", CurrentUserId, url, image.UserId);

                    throw new InvalidOperationException($"Image '{url}' belongs to another user and cannot be added.");
                }

                if (!entity.Images.Any(i => i.Id == image.Id))
                {
                    entity.Images.Add(image);
                    logger.LogInformation("Existing image {ImageId} added to entity", image.Id);
                }
            }
            else
            {
                entity.Images.Add(new Image
                {
                    Url = url,
                    UserId = CurrentUserId
                });

                logger.LogInformation("New image created and added. Url: {Url}, UserId: {UserId}", url, CurrentUserId);
            }
        }
    }

    public async Task AddImagesSafeAsync(IHasImages entity, List<string> urls)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        if (urls == null || urls.Count == 0)
            return;

        var distinctUrls = urls
            .Select(u => u.Trim())
            .Where(u => !string.IsNullOrWhiteSpace(u))
            .Distinct()
            .ToList();

        var existingImages = await imageRepository.GetAllByKeyAsync(img => distinctUrls.Contains(img.Url));

        foreach (var url in distinctUrls)
        {
            var image = existingImages.FirstOrDefault(i => i.Url == url);

            if (image != null)
            {
                if (image.UserId != null && image.UserId != CurrentUserId)
                {
                    logger.LogWarning(
                        "Skipped image {Url} – owned by another user ({OwnerId})", url, image.UserId);

                    continue;
                }

                if (!entity.Images.Any(i => i.Id == image.Id))
                    entity.Images.Add(image);
            }
            else
            {
                entity.Images.Add(new Image
                {
                    Url = url,
                    UserId = CurrentUserId
                });
            }
        }
    }


    public async Task<string?> RemoveImageFromEntityAsync<T>(T entity, int imageId, IRepository<T> entityRepository) where T : class, IHasImages
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var image = await imageRepository.GetByIdAsync(imageId);
        if (image == null)
        {
            logger.LogWarning("Attempted to remove non-existing image {ImageId}",imageId);
            throw new KeyNotFoundException($"Image with id {imageId} was not found.");
        }

        if (!entity.Images.Contains(image))
        {
            logger.LogWarning("Image {ImageId} is not attached to entity {EntityType}", imageId, typeof(T).Name);
            throw new InvalidOperationException($"Image {imageId} is not associated with this entity.");
        }

        var deletedUrl = image.Url;
        entity.Images.Remove(image);
        await entityRepository.UpdateAsync(entity);

        logger.LogInformation("Image {ImageId} removed from entity {EntityType}", imageId, typeof(T).Name);

        if (!image.Plants.Any() && !image.GrowthLogs.Any() && !image.Planted.Any() && !image.PlantExchanges.Any())
        {
            await imageRepository.DeleteAsync(image, false);
            logger.LogInformation(
                "Image {ImageId} deleted from repository (no remaining references)",
                imageId);

            return deletedUrl;
        }

        return null;
    }

}
