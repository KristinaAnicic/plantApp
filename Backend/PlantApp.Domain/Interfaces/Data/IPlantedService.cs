using PlantApp.Domain.Dtos.Planted;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantedService
{
    public Task<List<PlantedDto>> GetPlantedPlants(int userId);
    public Task<List<GroupedPlantedDto>> GetPlantedPlantsGroupedByPlace(int userId);
    public Task AddPlanted(UpsertPlantedDto dto);
    public Task UpdatePlanted(int id, UpsertPlantedDto dto);
    public Task DeletePlanted(int id);
}
