using Microsoft.Extensions.Logging;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Interfaces;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class GrowthLogService(
    IGrowthLogRepository repository,
    IRepository<PlantStatus> statusRepo,
    IPlantedRepository plantedRepo,
    IPlantGroupRepository groupRepo,
    IImageService imageService,
    ICurrentUserContext userContext,
    ILogger<GrowthLogService> logger
) : IGrowthLogService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;

    public async Task<List<GrowthLogDto>> GetAllAsync()
    {
        logger.LogInformation("Fetching all growth logs for user {UserId}", CurrentUserId);
        var logs = await repository.GetAllGrowthLogsByUserId(CurrentUserId);
        return logs.Select(l => l.MapGrowthLogToGrowthLogDto()).ToList();
    }

    public async Task<List<GrowthLogDto>> GetAllByPlantedIdAsync(int plantedId)
    {
        var planted = await plantedRepo.GetByIdAsync(plantedId);
        if (planted == null) return new();

        var logs = await repository.GetAllGrowthLogsByPlantedId(plantedId, planted.PlantGroupId);

        var log = logs.FirstOrDefault();

        if (log != null && planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
        {
            throw new UnauthorizedException("access", $"Planted {plantedId}", logger);
        }

        return logs.Select(l => l.MapGrowthLogToGrowthLogDto()).ToList();
    }

    public async Task<GrowthLogGetDto> GetByIdAsync(int id)
    {
        var log = await repository.GetGrowthLogById(id);
        EnsureLogExistsAndAuthorized(log);
        return log!.MapGrowthLogToGrowthLogGetDto();

    }

    public async Task AddAsync(UpsertGrowthLogDto dto)
    {
        if (dto.PlantGroupId == null && dto.PlantedId == null)
            throw new InvalidOperationAppException("Cannot add log without providing PlantGroupId or PlantedId", null, logger);

        var log = dto.MapUpsertGrowthLogDtoToGrowthLog();

        if (dto.PlantedId != null)
        {
            var planted = await plantedRepo.GetByIdAsync(dto.PlantedId.Value);
            if (planted == null)
                throw new NotFoundException("Planted", dto.PlantedId, logger);

            if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
                throw new UnauthorizedException("add log", $"Planted {dto.PlantedId}", logger);

            log.Planted.Add(planted);
            log.PlaceId = planted.PlaceId;
        }
        if (dto.PlantGroupId != null) {
            var group = await groupRepo.GetByIdAsync(dto.PlantGroupId.Value);
            if (group == null)
                throw new NotFoundException("Plant Group", dto.PlantGroupId, logger);

            if (group.UserId != CurrentUserId && !IsAdmin)
                throw new UnauthorizedException("add log", $"Plant Group {dto.PlantGroupId}", logger);

            foreach(var plant in group.PlantedList)
            {
                log.Planted.Add(plant);
            }
        }
      
        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new NotFoundException("Plant status", dto.PlantStatusId, logger);

        if (dto.Images != null && dto.Images.Any())
        {
            log.Images.Clear();
            await imageService.AddImagesSafeAsync(log, dto.Images);
        }
        

        await repository.AddAsync(log);
        logger.LogInformation("Growth log {LogId} successfully created", log.Id);
    }

    public async Task UpdateAsync(int id, UpsertGrowthLogDto dto)
    {
        if (id != dto.Id)
            throw new DtoIdMismatchException("GrowthLog", dto.Id, id, logger);

        if (dto.PlantGroupId == null && dto.PlantedId == null)
            throw new InvalidOperationAppException("Cannot add log without providing PlantGroupId or PlantedId", null, logger);

        var log = await repository.GetGrowthLogById(id);

        EnsureLogExistsAndAuthorized(log);

        if (dto.PlantedId != null)
        {
            var planted = await plantedRepo.GetByIdAsync(dto.PlantedId.Value);
            if (planted == null)
                throw new NotFoundException("Planted", dto.PlantedId, logger);

            if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
                throw new UnauthorizedException("modify log", $"Planted {dto.PlantedId}", logger);
        }
        if (dto.PlantGroupId != null)
        {
            var group = await groupRepo.GetByIdAsync(dto.PlantGroupId.Value);
            if (group == null)
                throw new NotFoundException("Plant Group", dto.PlantGroupId, logger);

            if (group.UserId != CurrentUserId && !IsAdmin)
                throw new UnauthorizedException("modify log", $"Plant Group {dto.PlantGroupId}", logger);
        }    

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new NotFoundException("Plant status", dto.PlantedId, logger);


        dto.MapUpsertGrowthLogDtoToGrowthLog(log);

        if (dto.Images != null && dto.Images.Any())
        {
            log!.Images.Clear();
            await imageService.AddImagesSafeAsync(log, dto.Images);
        }
        else
        {
            log!.Images.Clear();
        }

        await repository.UpdateAsync(log!);

        await imageService.RemoveUnusedImagesAsync();

        logger.LogInformation("Growth log {LogId} updated", id);
    }

    public async Task DeleteAsync(int id)
    {
        var log = await repository.GetGrowthLogById(id);
        EnsureLogExistsAndAuthorized(log);

        await repository.DeleteGrowthLog(log!);
        logger.LogInformation("Growth log {LogId} deleted", id);
    }

    public async Task AddImages(int logId, List<string> urls)
    {
        var log = await repository.GetGrowthLogById(logId);
        EnsureLogExistsAndAuthorized(log);

        await imageService.AddImagesToEntityAsync(log!, urls);
        await repository.UpdateAsync(log!);
    }

    public async Task RemoveImageById(int logId, int imageId)
    {
        var log = await repository.GetGrowthLogById(logId);
        EnsureLogExistsAndAuthorized(log);

        await imageService.RemoveImageFromEntityAsync(log!, imageId, repository);
    }

    private void EnsureLogExistsAndAuthorized(GrowthLog? log)
    {
        if (log == null)
            throw new NotFoundException("Growth log", logger: logger);

        bool isAuthorizedForPlanted = log.Planted.Any(p => p.Place.UserId == CurrentUserId);

        if (!isAuthorizedForPlanted && log.PlantGroup?.UserId != CurrentUserId && !IsAdmin)
        {
            throw new UnauthorizedException("access", $"GrowthLog {log.Id}", logger);
        }
    }
}
