using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security;

namespace PlantApp.Domain.Services.Data;

public class PlantedService(
    IPlantedRepository repository,
    IImageService imageService,
    IRepository<Place> placeRepo,
    IRepository<PlantStatus> plantStatus,
    ICurrentUserContext userContext,
    ILogger<PlantedService> logger
) : IPlantedService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    public async Task<List<PlantedDto>> GetAllByUserIdAsync(int userId)
    {
        logger.LogInformation("Fetching planted plants for user {UserId}",userId);

        var planted = await repository.GetPlantedPlantsByUserId(userId);
        return planted.Select(p => p.MapPlantedToPlantedDto()).ToList();
    }

    public async Task<List<GroupedPlantedDto>> GetAllByUserIdGroupedByPlaceAsync(int userId)
    {
        logger.LogInformation("Fetching planted plants grouped by place for user {UserId}", userId);

        var planted = await repository.GetPlantedPlantsByUserIdGrouped(userId);
        var groupedDto = planted
            .Select(g => new GroupedPlantedDto 
            { 
                Place = g.Key.MapPlaceToPlaceDto(), 
                Planted = g.Value.Select(p => p.MapPlantedToPlantedDto()).ToList()
            })
            .ToList();

        return groupedDto;
    }

    public async Task<PlantedGetDto> GetByIdAsync(int id)
    {
        var planted = await repository.GetPlantedById(id);

        if (planted == null)
        {
            logger.LogWarning("Planted plant {PlantedId} not found", id);
            throw new KeyNotFoundException($"Planted plant was not found.");
        }

        if (planted.Reminders != null && planted.Reminders.Any())
            planted.Reminders = planted.Reminders.OrderBy(r => (r.NextDueDate.AddDays(r.DelayDays) - DateTime.UtcNow).TotalDays).ToList();

        return planted.MapPlantedToPlantedGetDto();
    }

    public async Task AddAsync(UpsertPlantedDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.DatePlanted == null)
            dto.DatePlanted = DateTime.UtcNow;

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);
        if (place == null)
        {
            logger.LogWarning("Place {PlaceId} not found while adding planted plant", dto.PlaceId);
            throw new KeyNotFoundException($"Place was not found.");
        }

        if (place.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to add plant to place {PlaceId} owned by {OwnerId}", CurrentUserId, place.Id, place.UserId);
            throw new SecurityException("You are not authorized to add plants to this place.");
        }

        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId))
        {
            logger.LogWarning("Invalid plant status {PlantStatusId}",dto.PlantStatusId);
            throw new KeyNotFoundException($"Plant status was not found.");
        }

        var planted = dto.MapUpsertPlantedDtoToPlanted();

        if (dto.Images != null && dto.Images.Any())
        {
            planted.Images.Clear();

            await imageService.AddImagesSafeAsync(planted, dto.Images);
        }

        await repository.AddAsync(planted);

        logger.LogInformation("Planted plant added. User {UserId}, Place {PlaceId}", CurrentUserId, dto.PlaceId);
    }

    public async Task UpdateAsync(int id, UpsertPlantedDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (dto.Id != id)
            throw new ArgumentException("DTO id does not match route id.");

        var existingPlanted = await repository.GetByIdAsync(id);

        if (existingPlanted == null)
        {
            logger.LogWarning("Attempt to update non-existing planted plant {PlantedId}", id);
            throw new KeyNotFoundException($"Planted plant with id {id} does not exist.");
        }

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);
        if (place == null)
        {
            throw new KeyNotFoundException($"Place with id {dto.PlaceId} was not found.");
        }

        if (place.UserId != CurrentUserId)
            throw new SecurityException("You are not authorized to update plants in this place.");

        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId))
        {
            throw new KeyNotFoundException("Plant status was not found.");
        }

        if (dto.DatePlanted == null)
            dto.DatePlanted = existingPlanted.DatePlanted;

        dto.MapUpsertPlantedDtoToPlanted(existingPlanted);

        if (dto.Images != null && dto.Images.Any())
        {
            existingPlanted.Images.Clear();
            await imageService.AddImagesSafeAsync(existingPlanted, dto.Images);
        }

        await repository.UpdateAsync(existingPlanted);

        logger.LogInformation("Planted plant {PlantedId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var planted = await repository.GetByIdAsync(id);

        if (planted == null)
        {
            logger.LogWarning("Attempt to delete non-existing planted plant {PlantedId}", id);
            throw new KeyNotFoundException($"Planted plant with id {id} does not exist.");
        }

        await repository.DeletePlantedAsync(planted);

        logger.LogInformation("Planted plant {PlantedId} deleted by user {UserId}", id, CurrentUserId);
    }

    public async Task AddImages(int plantedId, List<string> urls)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null)
        {
            throw new KeyNotFoundException($"Planted plant was not found.");
        }

        await imageService.AddImagesToEntityAsync(planted, urls);
        await repository.UpdateAsync(planted);

        logger.LogInformation("Images added to planted plant {PlantedId}", plantedId);
    }

    public async Task<string?> RemoveImageById(int plantedId, int imageId)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null)
            throw new KeyNotFoundException("Plant not found");

        if (planted.Place != null && planted.Place.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to remove image {ImageId} from planted {PlantedId}", CurrentUserId, imageId, plantedId);
            throw new SecurityException("Access denied.");
        }

        var deletedUrl = await imageService.RemoveImageFromEntityAsync(planted, imageId, repository);      
        //await repository.UpdateAsync(planted);

        return deletedUrl;
    }
}
