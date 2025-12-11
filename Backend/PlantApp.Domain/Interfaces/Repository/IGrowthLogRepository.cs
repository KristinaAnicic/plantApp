using PlantApp.Data.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IGrowthLogRepository : IRepository<GrowthLog>
{
    public Task<List<GrowthLog>> GetAllGrowthLogsByUserId(int userId);
    public Task<List<GrowthLog>> GetAllGrowthLogsByPlantedId(int plantedId);
    public Task<GrowthLog?> GetGrowthLogById(int id);
    public Task DeleteGrowthLog(GrowthLog log);
}
