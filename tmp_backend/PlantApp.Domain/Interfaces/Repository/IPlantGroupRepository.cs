using PlantApp.Domain.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantGroupRepository : IRepository<PlantGroup>
{
    public Task<PlantGroup?> GetPlantGroupById(int id);
}
