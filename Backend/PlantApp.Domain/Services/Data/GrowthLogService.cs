using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class GrowthLogService(
    IGrowthLogRepository repository,
    IRepository<PlantStatus> statusRepo,
    IPlantedRepository plantedRepo,
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
        var logs = await repository.GetAllGrowthLogsByPlantedId(plantedId);

        var log = logs.FirstOrDefault();

        if (log != null && log.Planted != null && log.Planted.Place != null && log.Planted.Place.UserId != CurrentUserId && !IsAdmin)
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

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId);
        if (planted == null)
            throw new NotFoundException("Planted", dto.PlantedId, logger);

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new NotFoundException("Plant status", dto.PlantStatusId, logger);

        if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("add log", $"Planted {dto.PlantedId}", logger);        

        var log = dto.MapUpsertGrowthLogDtoToGrowthLog();

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

        var log = await repository.GetGrowthLogById(id);

        EnsureLogExistsAndAuthorized(log);

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId);
        if (planted == null)
            throw new NotFoundException("Planted", dto.PlantedId, logger);

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new NotFoundException("Planted", dto.PlantedId, logger);

        if (planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("modify log", $"Planted {dto.PlantedId}", logger);

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

        if (log.Planted?.Place?.UserId != CurrentUserId && !IsAdmin)
        {
            throw new UnauthorizedException("access", $"GrowthLog {log.Id}", logger);
        }
    }
}
