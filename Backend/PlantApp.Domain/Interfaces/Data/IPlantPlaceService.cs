using PlantApp.Domain.Dtos.PlantPlace;

namespace PlantApp.Domain.Interfaces.Data;

public interface IPlantPlaceService
{
    public Task<List<PlaceDto>> GetAllPlaces(int userId);
    public Task<PlaceGetDto> GetPlaceById(int id);
    public Task AddPlace(UpsertPlaceDto dto);
    public Task UpdatePlace(int id, UpsertPlaceDto place);
    public Task DeletePlace(int id);
}
