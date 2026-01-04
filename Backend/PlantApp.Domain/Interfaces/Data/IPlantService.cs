using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantService
{
    public Task<ListResponse<PlantDto>> GetAllAsync(int page);
    public Task<PlantGetDto?> GetByIdAsync(int id);
    public Task<ListResponse<PlantDto>> GetFilteredAsync(FilterByDto filter, int page);
    public Task AddAsync(UpsertPlantDto plantDto);
    public Task UpdateAsync(int Id, UpsertPlantDto plantDto);
    public Task DeleteAsync(int Id);
    public Task AddImages(int plantId, List<string> urls);
    public Task<string?> RemoveImageById(int plantId, int imageId);
    public Task<ManyPlantAttributesDto> GetMultiReferenceDataAsync();
    public Task<OnePlantAttributesDto> GetSinglePlantReferenceDataAsync();
}
