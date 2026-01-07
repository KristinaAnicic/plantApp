using PlantApp.Domain.Dtos.PlantPlace;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantPlaceService
{
    public Task<List<PlaceDto>> GetAllAsync();
    public Task<PlaceGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlaceDto dto);
    public Task UpdateAsync(int id, UpsertPlaceDto place);
    public Task DeleteAsync(int id);
}
