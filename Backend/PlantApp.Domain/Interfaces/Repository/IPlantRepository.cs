using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantRepository : IRepository<Plant>
{
    public Task<(int, List<Plant>)> GetPlantsFiltered(FilterByDto filter, int page);
    public Task<List<Plant>> GetAllPlantsAsync(int page);
}
