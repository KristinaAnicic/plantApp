using PlantApp.Domain.Dtos.GrowthLog;

namespace PlantApp.Domain.Interfaces.Data;

public interface IGrowthLogService
{
    public Task<List<GrowthLogDto>> GetAllAsync();
    public Task<List<GrowthLogDto>> GetAllByPlantedIdAsync(int plantedId);
    public Task<GrowthLogGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertGrowthLogDto dto);
    public Task UpdateAsync(int id, UpsertGrowthLogDto dto);
    public Task DeleteAsync(int id);
    public Task AddImages(int logId, List<string> urls);
    public Task RemoveImageById(int logId, int imageId);
}
