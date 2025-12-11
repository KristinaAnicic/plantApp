using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantExchangeService
{
    public Task<List<PlantExchangeDto>> GetActiveAsync();
    public Task<List<PlantExchangeDto>> GetActiveFilteredAsync(PlantExchangeFilterDto filter);
    public Task<PlantExchangeGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlantExchangeDto dto);
    public Task UpdateAsync(int id, UpsertPlantExchangeDto dto);
    public Task DeleteAsync(int id);
}
