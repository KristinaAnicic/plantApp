using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

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

        if (log != null && log.Planted != null && log.Planted.Place != null && log.Planted.Place.UserId != CurrentUserId)
        {
            logger.LogWarning(
                "Unauthorized access to plantedId {PlantedId} by user {UserId}",
                plantedId, CurrentUserId);
            throw new UnauthorizedAccessException("You do not have access to this planted.");
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
            throw new KeyNotFoundException("Planted not found.");

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new KeyNotFoundException("Plant status not found.");

        if (planted.Place != null && planted.Place.UserId != CurrentUserId)
        {
            logger.LogWarning(
                "User {UserId} tried to add log to foreign planted {PlantedId}",
                CurrentUserId, dto.PlantedId);

            throw new UnauthorizedAccessException("You cannot add logs to this planted.");
        }

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
            throw new ArgumentException("Route id does not match DTO id.");

        var log = await repository.GetGrowthLogById(id);

        EnsureLogExistsAndAuthorized(log);

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId);
        if (planted == null)
            throw new KeyNotFoundException("Planted not found.");

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new KeyNotFoundException("Plant status not found.");

        if (planted.Place != null && planted.Place.UserId != CurrentUserId)
            throw new UnauthorizedAccessException("You cannot modify this planted.");

        dto.MapUpsertGrowthLogDtoToGrowthLog(log);

        if (dto.Images != null && dto.Images.Any())
        {
            log!.Images.Clear();
            await imageService.AddImagesSafeAsync(log, dto.Images);
        }

        await repository.UpdateAsync(log!);

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
        var log = await repository.GetByIdAsync(logId);
        EnsureLogExistsAndAuthorized(log);

        await imageService.AddImagesToEntityAsync(log!, urls);

        await repository.UpdateAsync(log!);
    }

    public async Task<string?> RemoveImageById(int logId, int imageId)
    {
        var log = await repository.GetGrowthLogById(logId);
        EnsureLogExistsAndAuthorized(log);

        var deletedUrl = await imageService.RemoveImageFromEntityAsync(log!, imageId, repository);
        //await repository.UpdateAsync(log!);

        return deletedUrl;
    }

    private void EnsureLogExistsAndAuthorized(GrowthLog? log)
    {
        if (log == null)
            throw new KeyNotFoundException("Growth log not found.");

        if (log.Planted?.Place?.UserId != CurrentUserId)
        {
            logger.LogWarning(
                "Unauthorized access to growth log {LogId} by user {UserId}",
                log.Id, CurrentUserId);

            throw new UnauthorizedAccessException("You do not have access to this growth log.");
        }
    }
}
