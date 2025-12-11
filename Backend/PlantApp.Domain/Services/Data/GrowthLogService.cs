using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Numerics;

namespace PlantApp.Domain.Services.Data;

public class GrowthLogService(
    IGrowthLogRepository repository,
    IRepository<PlantStatus> statusRepo,
    IRepository<Planted> plantedRepo,
    IImageService imageService
) : IGrowthLogService
{
    public int currentUser = 0;

    public async Task<List<GrowthLogDto>> GetAllAsync()
    {
        var logs = await repository.GetAllGrowthLogsByUserId(currentUser);
        return logs.Select(l => l.MapGrowthLogToGrowthLogDto()).ToList();
    }

    public async Task<List<GrowthLogDto>> GetAllByPlantedIdAsync(int plantedId)
    {
        var logs = await repository.GetAllGrowthLogsByPlantedId(plantedId);

        var log = logs.FirstOrDefault();

        if (log != null && log.Planted != null && log.Planted.Place != null && log.Planted.Place.UserId != currentUser)
            throw new ArgumentException("Access to planted denied");

        return logs.Select(l => l.MapGrowthLogToGrowthLogDto()).ToList();
    }

    public async Task<GrowthLogGetDto> GetByIdAsync(int id)
    {
        var log = await repository.GetGrowthLogById(id);

        CheckLogAndAuthorization(log);
        return log.MapGrowthLogToGrowthLogGetDto();

    }

    public async Task AddAsync(UpsertGrowthLogDto dto)
    {
        if (!await plantedRepo.IdExistsAsync(dto.PlantedId))
            throw new ArgumentException("Unknown planted");

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new ArgumentException("Unknown plant status");

        var log = dto.MapUpsertGrowthLogDtoToGrowthLog();

        if (dto.Images != null && dto.Images.Any())
        {
            log.Images.Clear();

            await imageService.AddImagesSafeAsync(log, dto.Images);
        }

        await repository.AddAsync(log);
    }

    public async Task UpdateAsync(int id, UpsertGrowthLogDto dto)
    {
        if (id != dto.Id)
            throw new ArgumentException("DTO id does not match provided id");
        
        var log = await repository.GetGrowthLogById(id);

        CheckLogAndAuthorization(log);

        if (!await plantedRepo.IdExistsAsync(dto.PlantedId))
            throw new ArgumentException("Unknown planted");

        if (!await statusRepo.IdExistsAsync(dto.PlantStatusId))
            throw new ArgumentException("Unknown plant status");

        dto.MapUpsertGrowthLogDtoToGrowthLog(log);

        if (dto.Images != null && dto.Images.Any())
        {
            log!.Images.Clear();

            await imageService.AddImagesSafeAsync(log, dto.Images);
        }

        await repository.UpdateAsync(log!);
    }

    public async Task DeleteAsync(int id)
    {
        var log = await repository.GetGrowthLogById(id);
        CheckLogAndAuthorization(log);

        await repository.DeleteGrowthLog(log!);
    }

    public async Task AddImages(int logId, List<string> urls)
    {
        var log = await repository.GetByIdAsync(logId);
        CheckLogAndAuthorization(log);

        await imageService.AddImagesToEntityAsync(log!, urls);

        await repository.UpdateAsync(log!);
    }

    public async Task RemoveImageById(int logId, int imageId)
    {
        var log = await repository.GetGrowthLogById(logId);
        CheckLogAndAuthorization(log);

        await imageService.RemoveImageFromEntityAsync(log!, imageId);
        await repository.UpdateAsync(log!);
    }

    private void CheckLogAndAuthorization(GrowthLog? log)
    {
        if (log == null)
            throw new ArgumentException("Log not found");

        if (log.Planted != null && log.Planted.Place != null && log.Planted.Place.UserId != currentUser)
            throw new ArgumentException("Access to log denied");
    }
}
