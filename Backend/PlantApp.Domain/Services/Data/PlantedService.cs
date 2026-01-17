using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

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
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<List<PlantedDto>> GetAllByUserIdAsync(int? userId)
    {
        int actualUserId = userId ?? CurrentUserId;
        if (actualUserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("fetching plants in", "planted", logger);

        logger.LogInformation("Fetching planted plants for user {UserId}", actualUserId);
        var planted = await repository.GetPlantedPlantsByUserId(actualUserId);
        return planted.Select(p => p.MapPlantedToPlantedDto()).ToList();
    }

    public async Task<List<GroupedPlantedDto>> GetAllByUserIdGroupedByPlaceAsync(int? userId)
    {
        int actualUserId = userId ?? CurrentUserId;
        if (actualUserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("fetching plants in", "planted", logger);

        logger.LogInformation("Fetching planted plants grouped by place for user {UserId}", actualUserId);

        var planted = await repository.GetPlantedPlantsByUserIdGrouped(actualUserId);
        var groupedDto = planted
            .Select(g => new GroupedPlantedDto 
            { 
                Place = g.Key.MapPlaceToPlaceDto(), 
                Planted = g.Value.Select(p => p.MapPlantedToPlantedDto()).ToList()
            })
            .ToList();

        return groupedDto;
    }

    public async Task<PlaceGetDto> GetAllByPlaceIdAsync(int placeId)
    {
        var place = await placeRepo.GetByIdAsync(placeId);
        if (place == null)
            throw new NotFoundException("Place", placeId, logger);
        
        if (place.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("fetching plants in", "planted", logger);

        logger.LogInformation("Fetching planted plants by place for user {UserId}", place.UserId);

        var planted = await repository.GetPlantedPlantsByPlaceId(placeId);
        var dto = place.MapPlaceToPlaceGetDto();
        dto.Planted = planted.Select(p => p.MapPlantedToPlantedDto()).ToList();

        return dto;
    }

    public async Task<PlantedGetDto> GetByIdAsync(int id)
    {
        var planted = await repository.GetPlantedById(id);

        if (planted == null) 
            throw new NotFoundException("Planted plant", id, logger);

        if (planted.Reminders != null && planted.Reminders.Any()) 
        { 
            planted.Reminders = planted.Reminders
                .OrderBy(r => (r.NextDueDate.AddDays(r.DelayDays) - DateTime.UtcNow).TotalDays)
                .ToList();
        }

        return planted.MapPlantedToPlantedGetDto();
    }

    public async Task AddAsync(UpsertPlantedDto dto)
    {
        if (dto == null) 
            throw new InvalidOperationAppException("Planted data is required.", logger: logger);

        if (dto.DatePlanted == null)
            dto.DatePlanted = DateTime.UtcNow;

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);

        if (place == null) 
            throw new NotFoundException("Place", dto.PlaceId, logger);
        if (place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("add plant to", "place", logger);
        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId)) 
            throw new NotFoundException("Plant status", dto.PlantStatusId, logger);

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
            throw new InvalidOperationAppException("Planted data is required.", logger: logger);
        if (dto.Id != id) 
            throw new DtoIdMismatchException("Planted plant", dto.Id ?? 0, id, logger);

        var existingPlanted = await repository.GetByIdAsync(id);
        if (existingPlanted == null) 
            throw new NotFoundException("Planted plant", id, logger);

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);

        if (place == null) 
            throw new NotFoundException("Place", dto.PlaceId, logger);
        if (place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update plants in", "place", logger);
        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId)) 
            throw new NotFoundException("Plant status", dto.PlantStatusId, logger);

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
            throw new NotFoundException("Planted plant", id, logger);

        await repository.DeletePlantedAsync(planted);
        logger.LogInformation("Planted plant {PlantedId} deleted by user {UserId}", id, CurrentUserId);
    }

    public async Task AddImages(int plantedId, List<string> urls)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null) 
            throw new NotFoundException("Planted plant", plantedId, logger);

        await imageService.AddImagesToEntityAsync(planted, urls);
        await repository.UpdateAsync(planted);

        logger.LogInformation("Images added to planted plant {PlantedId}", plantedId);
    }

    public async Task<string?> RemoveImageById(int plantedId, int imageId)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null) 
            throw new NotFoundException("Planted plant", plantedId, logger);

        if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("remove image", "planted plant", logger);

        var deletedUrl = await imageService.RemoveImageFromEntityAsync(planted, imageId, repository);      
        //await repository.UpdateAsync(planted);

        return deletedUrl;
    }
}
