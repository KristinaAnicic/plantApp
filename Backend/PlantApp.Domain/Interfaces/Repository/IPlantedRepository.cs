using PlantApp.Domain.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantedRepository : IRepository<Planted>
{
    public Task<List<Planted>> GetPlantedPlantsByUserId(int userId, bool filterByName = false);
    public Task<int> GetNumOfDeadPlants(int userId);
    public Task<List<Planted>> GetAllDeadPlantsAsync(int userId);
    public Task<Planted?> GetPlantedById(int id);
    public Task<List<Planted>> GetPlantedPlantsByPlaceId(int placeId);
    public Task<Dictionary<Place, List<Planted>>> GetPlantedPlantsByUserIdGrouped(int userId, bool filterByName = false);
    public Task DeletePlantedAsync(Planted planted);
}
