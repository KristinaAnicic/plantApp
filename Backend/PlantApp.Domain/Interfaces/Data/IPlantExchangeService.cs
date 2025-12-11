using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantExchangeService
{
    public Task<ListResponse<PlantExchangeDto>> GetActiveAsync(int page);
    public Task<ListResponse<PlantExchangeDto>> GetActiveFilteredAsync(PlantExchangeFilterDto filter, int page);
    public Task<PlantExchangeGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlantExchangeDto dto);
    public Task UpdateAsync(int id, UpsertPlantExchangeDto dto);
    public Task DeleteAsync(int id);
    public Task AddImages(int exchangeId, List<string> urls);
    public Task RemoveImageById(int exchangeId, int imageId);
}
