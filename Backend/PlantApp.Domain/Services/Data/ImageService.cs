using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Data.Models.Interfaces;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class ImageService(
    IRepository<Image> imageRepository,
    ICurrentUserContext userContext,
    ILogger<ImageService> logger
) : IImageService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task AddImagesToEntityAsync(IHasImages entity, List<string> urls)
    {
        if (entity == null) 
        {
            throw new InvalidOperationAppException(
                userMessage: "Entity is required for adding images.",
                internalMessage: "Null entity provided to AddImagesToEntityAsync.",
                logger: logger
            );
        }

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
                if (image.UserId != null && image.UserId != CurrentUserId && !IsAdmin) 
                    throw new UnauthorizedException("add image", $"Image {url}", logger);            

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
        {    
            throw new InvalidOperationAppException(
                userMessage: "Entity is required for adding images.",
                internalMessage: "Null entity provided to AddImagesSafeAsync.",
                logger: logger
            );
        }

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
                if (image.UserId != null && image.UserId != CurrentUserId && !IsAdmin)
                {
                    logger.LogWarning("Skipped image {Url} – owned by another user ({OwnerId})", url, image.UserId);
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
        {
            throw new InvalidOperationAppException(
                userMessage: "Entity is required for removing an image.",
                internalMessage: "Null entity provided to RemoveImageFromEntityAsync.",
                logger: logger
            );
        }

        var image = await imageRepository.GetByIdAsync(imageId);
        if (image == null) 
            throw new NotFoundException("Image", imageId, logger);

        if (!entity.Images.Contains(image))
        {
            throw new InvalidOperationAppException(
                userMessage: "The image is not associated with this entity.",
                internalMessage: $"Image {imageId} not attached to entity {typeof(T).Name}.",
                logger: logger
            );
        }

        var deletedUrl = image.Url;
        entity.Images.Remove(image);
        await entityRepository.UpdateAsync(entity);

        logger.LogInformation("Image {ImageId} removed from entity {EntityType}", imageId, typeof(T).Name);

        if (!image.Plants.Any() && !image.GrowthLogs.Any() && !image.Planted.Any() && !image.PlantExchanges.Any())
        {
            await imageRepository.DeleteAsync(image, false);
            logger.LogInformation("Image {ImageId} deleted from repository (no remaining references)",imageId);

            return deletedUrl;
        }

        return null;
    }

}
