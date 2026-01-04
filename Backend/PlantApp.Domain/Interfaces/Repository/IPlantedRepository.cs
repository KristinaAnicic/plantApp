using PlantApp.Data.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantedRepository : IRepository<Planted>
{
    public Task<List<Planted>> GetPlantedPlantsByUserId(int userId, bool filterByName = false);
    public Task<Planted?> GetPlantedById(int id);
    public Task<Dictionary<Place, List<Planted>>> GetPlantedPlantsByUserIdGrouped(int userId, bool filterByName = false);
    public Task DeletePlantedAsync(Planted planted);
}
