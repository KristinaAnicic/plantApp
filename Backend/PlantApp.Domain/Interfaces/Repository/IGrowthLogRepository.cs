using PlantApp.Domain.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IGrowthLogRepository : IRepository<GrowthLog>
{
    public Task<List<GrowthLog>> GetAllGrowthLogsByUserId(int userId);
    public Task<List<GrowthLog>> GetAllGrowthLogsByPlantedId(int plantedId, int? plantGroupId);
    public Task<List<GrowthLog>> GetAllGrowthLogsByPlantGroupId(int plantGroupId);
    public Task<GrowthLog?> GetGrowthLogById(int id);
    public Task DeleteGrowthLog(GrowthLog log);
}
