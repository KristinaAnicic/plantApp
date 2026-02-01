using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
using PlantApp.Domain.Models.Interfaces;

namespace PlantApp.Domain.Services.Data;

public class PlantedService(
    IPlantedRepository repository,
    IImageService imageService,
    IPlantRepository plantRepo,
    IRepository<Place> placeRepo,
    IRepository<PlantStatus> plantStatusRepo,
    ICurrentUserContext userContext,
    ILogger<PlantedService> logger
) : IPlantedService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<PlantedWithAnyDeadBoolDto> GetAllByUserIdAsync(int? userId)
    {
        int actualUserId = userId ?? CurrentUserId;
        if (actualUserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("fetching plants in", "planted", logger);

        logger.LogInformation("Fetching planted plants for user {UserId}", actualUserId);
        var planted = await repository.GetPlantedPlantsByUserId(actualUserId);
        var numOfDeadPlants = await repository.GetNumOfDeadPlants(actualUserId);

        return new PlantedWithAnyDeadBoolDto { NumOfDeadPlants = numOfDeadPlants, Planted = planted.Select(p => p.MapPlantedToPlantedDto()).ToList() };
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

    public async Task<List<PlantedDto>> GetAllDeadPlantsAsync(int? userId)
    {
        int actualUserId = userId ?? CurrentUserId;
        if (actualUserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("fetching dead plants in", "planted", logger);

        logger.LogInformation("Fetching dead plants for user {UserId}", actualUserId);
        var planted = await repository.GetAllDeadPlantsAsync(actualUserId);
        return planted.Select(p => p.MapPlantedToPlantedDto()).ToList();
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
                .OrderBy(r => r.NextDueDate)
                .ToList();
        }

        if (planted.GrowthLogs != null && planted.GrowthLogs.Any())
        {
            planted.GrowthLogs = planted.GrowthLogs
                .OrderByDescending(r => r.ObservationDate)
                .ThenByDescending(r => r.CreatedAt)
                .ToList();
        }

        return planted.MapPlantedToPlantedGetDto();
    }

    public async Task AddAsync(UpsertPlantedDto dto)
    {
        if (dto == null) 
            throw new InvalidOperationAppException("Planted data is required.", logger: logger);

        if (dto.DatePlanted == null)
            dto.DatePlanted = DateOnly.FromDateTime(DateTime.UtcNow);

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);

        if (place == null) 
            throw new NotFoundException("Place", dto.PlaceId, logger);
        if (place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("add plant to", "place", logger);
        if (!await plantStatusRepo.IdExistsAsync(dto.PlantStatusId)) 
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
        if (!await plantStatusRepo.IdExistsAsync(dto.PlantStatusId)) 
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

        await imageService.RemoveUnusedImagesAsync();

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

    public async Task RemoveImageById(int plantedId, int imageId)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null) 
            throw new NotFoundException("Planted plant", plantedId, logger);

        if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("remove image", "planted plant", logger);

        await imageService.RemoveImageFromEntityAsync(planted, imageId, repository);      
    }

    public async Task<PlantedReferences> GetReferences()
    {
        var places = await placeRepo.GetAllByKeyAsync(p => p.UserId == CurrentUserId);
        var plantStatuses = await plantStatusRepo.GetAllAsync();

        return new PlantedReferences
        {
            Places = places.Select(p => p.MapReferenceToDto()).ToList(),
            PlantStatuses = plantStatuses.Select(s => s.MapReferenceToDto()).ToList()
        };
    }
}
