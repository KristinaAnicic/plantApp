using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantService
{
    public Task<List<PlantDto>> GetAllAsync();
    public Task<PlantGetDto?> GetByIdAsync(int id);
    public Task<List<PlantDto>> GetFilteredAsync(FilterByDto filter);
    public Task AddAsync(UpsertPlantDto plantDto);
    public Task UpdateAsync(int Id, UpsertPlantDto plantDto);
    public Task DeleteAsync(int Id);
    public Task AddImages(int plantId, List<string> urls);
    public Task RemoveImageById(int plantId, int imageId);
    public Task<ManyPlantAttributesDto> GetMultiReferenceDataAsync();
    public Task<OnePlantAttributesDto> GetSinglePlantReferenceDataAsync();
}
