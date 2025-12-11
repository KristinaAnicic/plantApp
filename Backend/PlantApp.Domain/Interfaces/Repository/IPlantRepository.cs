using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantRepository : IRepository<Plant>
{
    public Task<List<Plant>> GetPlantsFiltered(FilterByDto filter, int page);
    public Task<List<Plant>> GetAllPlantsAsync(int page);
}
