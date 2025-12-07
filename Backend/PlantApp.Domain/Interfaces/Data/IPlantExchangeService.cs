using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantExchangeService
{
    public Task<List<PlantExchangeDto>> GetActivePlantExchanges();
    public Task<List<PlantExchangeDto>> GetActivePlantsFiltered(PlantExchangeFilterDto filter);
    public Task<PlantExchangeGetDto> GetPlantExchange(int id);
    public Task AddPlantExchange(UpsertPlantExchangeDto dto);
    public Task UpdatePlantExchange(int id, UpsertPlantExchangeDto dto);
    public Task DeletePlantExchange(int id);
}
