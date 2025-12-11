using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantedService
{
    public Task<List<PlantedDto>> GetAllByUserIdAsync(int userId);
    public Task<List<GroupedPlantedDto>> GetAllByUserIdGroupedByPlaceAsync(int userId);
    public Task<PlantedGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlantedDto dto);
    public Task UpdateAsync(int id, UpsertPlantedDto dto);
    public Task DeleteAsync(int id);
    public Task AddImages(int plantedId, List<string> urls);
    public Task RemoveImageById(int plantedId, int imageId);
}
