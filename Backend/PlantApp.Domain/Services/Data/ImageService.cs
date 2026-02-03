using Appwrite;
using Appwrite.Services;
using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Interfaces;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class ImageService(
    IRepository<Image> imageRepository,
    ICurrentUserContext userContext,
    ILogger<ImageService> logger,
    Client appWriteClient
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

    public async Task RemoveUnusedImagesAsync()
    {
        var unusedImages = await imageRepository
            .GetAllByKeyAsync(im =>
            !im.Plants.Any() &&
            !im.Planted.Any() &&
            !im.GrowthLogs.Any() &&
            !im.PlantExchanges.Any());

        if (unusedImages != null && unusedImages.Any()) {
            var storage = new Storage(appWriteClient);
            var bucketId = "697113c5003c549608f1";

            foreach (var img in unusedImages) { 
                try
                {
                    if (string.IsNullOrEmpty(img.Url) || !img.Url.Contains("appwrite")) continue;

                    string fileId = GetFileIdFromAppwriteUrl(img.Url);
                    await storage.DeleteFile(bucketId, fileId);
                    logger.LogInformation($"Deleted image from Appwrite: {fileId}");
                }
                catch (Exception ex){
                    logger.LogError($"Appwrite delete failed for image {img.Url}: {ex.Message}");
                }
            }
            await imageRepository.DeleteRangeAsync(unusedImages, false);
        }
    }

    private string GetFileIdFromAppwriteUrl(string url)
    {
        var uri = new Uri(url);
        var segments = uri.Segments;
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i].Contains("files/"))
            {
                return segments[i + 1].TrimEnd('/');
            }
        }
        return string.Empty;
    }

    public async Task RemoveImageFromEntityAsync<T>(T entity, int imageId, IRepository<T> entityRepository) where T : class, IHasImages
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

        entity.Images.Remove(image);
        await entityRepository.UpdateAsync(entity);

        await RemoveUnusedImagesAsync();

        logger.LogInformation("Image {ImageId} removed from entity {EntityType}", imageId, typeof(T).Name); 
    }

}
