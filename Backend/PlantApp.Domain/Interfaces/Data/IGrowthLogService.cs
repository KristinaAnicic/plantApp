using PlantApp.Domain.Dtos.GrowthLog;

namespace PlantApp.Domain.Interfaces.Data;

public interface IGrowthLogService
{
    public Task<List<GrowthLogDto>> GetGrowthLogs();
    public Task<List<GrowthLogDto>> GetGrowthLogByPlantedId(int plantedId);
    public Task<GrowthLogGetDto> GetGrowthLogById(int id);
    public Task AddGrowthLog(UpsertGrowthLogDto dto);
    public Task UpdateGrowthLog(int id, UpsertGrowthLogDto dto);
    public Task DeleteGrowthLog(int id);
}
