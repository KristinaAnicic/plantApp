using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IPlantExchangeRepository : IRepository<PlantExchange>
{
    public Task<(int, List<PlantExchange>)> GetActivePlantExchanges(int page);
    public Task<(int, List<PlantExchange>)> GetPlantExchangesFiltered(PlantExchangeFilterDto filter, int page);
    public Task<PlantExchange?> GetPlantExchangeById(int id);
}
