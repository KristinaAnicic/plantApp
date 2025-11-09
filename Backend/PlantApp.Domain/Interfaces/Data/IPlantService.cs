using PlantApp.Domain.Dtos.Plant;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantService
{
    public Task<List<PlantDto>> GetAllPlants();
    public Task<PlantGetDto?> GetPlantById(int id);
    public Task<List<PlantDto>> GetPlantsByName(FilterByDto filter, string? name = null);
    public Task InsertPlant(UpsertPlantDto plantDto);
    public Task UpdatePlant(UpsertPlantDto plantDto, int Id);
    public Task DeletePlantAsync(int Id);
    public Task<ManyPlantAttributesDto> GetMultiReferenceData();
    public Task<OnePlantAttributesDto> GetSinglePlantReferenceData();
}
