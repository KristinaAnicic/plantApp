using PlantApp.Domain.Dtos.PlantGroup;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantGroupService
{
    public Task<List<PlantGroupDto>> GetAllAsync();
    public Task<PlantGroupGetDto> GetByIdAsync(int id);
    public Task AddAsync(UpsertPlantGroupDto dto);
    public Task UpdateAsync(int id, UpsertPlantGroupDto dto);
    public Task DeleteAsync(int id);
    public Task AddPlantsToGroupAsync(int id, List<int> plants);
    public Task AddPlantToGroupAsync(int id, int plantId);
    public Task RemovePlantFromGroupAsync(int plantId);
}
