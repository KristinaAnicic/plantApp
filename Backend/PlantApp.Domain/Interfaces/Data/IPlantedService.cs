using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.PlantPlace;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantedService
{
    public Task<PlantedWithAnyDeadBoolDto> GetAllByUserIdAsync(int? userId);
    public Task<List<GroupedPlantedDto>> GetAllByUserIdGroupedByPlaceAsync(int? userId);
    public Task<PlaceGetDto> GetAllByPlaceIdAsync(int placeId);
    public Task<List<PlantedDto>> GetAllDeadPlantsAsync(int? userId);
    public Task<PlantedGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlantedDto dto);
    public Task UpdateAsync(int id, UpsertPlantedDto dto);
    public Task DeleteAsync(int id);
    public Task AddImages(int plantedId, List<string> urls);
    public Task RemoveImageById(int plantedId, int imageId);
    public Task<PlantedReferences> GetReferences();
}
