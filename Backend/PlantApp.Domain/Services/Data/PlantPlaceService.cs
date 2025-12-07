using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security.Authentication;

namespace PlantApp.Domain.Services.Data;

public class PlantPlaceService(
    IRepository<Place> repository,
    IRepository<Country> countryRepository
) : IPlantPlaceService
{
    int currentUser = 0;
    public async Task<List<PlaceDto>> GetAllPlaces(int userId)
    {
        var places = await repository.GetAllByKeyAsync(p => p.UserId == userId, true);
        places.OrderBy(p => p.Name)
            .ThenBy(p => p.Country != null ? p.Country.Name : p.City)
            .ThenBy(p => p.City)
            .ThenBy(p => p.Address);

        return places.Select(p => p.MapPlaceToPlaceDto()).ToList();
    }

    public async Task<PlaceGetDto> GetPlaceById(int id)
    {
        int currentUser = 0;
        var place = await repository.GetByIdAsync(id);

        if (place == null)
            throw new ArgumentException("Place with provided ID does not exist");

        if (place.UserId != currentUser)
            throw new AuthenticationException("Request to place is denied");

        return place.MapPlaceToPlaceGetDto();
    }

    public async Task AddPlace(UpsertPlaceDto dto)
    {
        var country = countryRepository.GetByIdAsync(dto.CountryId);
        if (country == null)
            throw new ArgumentException("Country does not exist");

        var place = dto.MapUpsertPlaceDtoToPlace();
        
        place.UserId = currentUser;
        await repository.AddAsync(place);
    }

    public async Task UpdatePlace(int id, UpsertPlaceDto place)
    {
        if (id != place.Id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingPlace = await repository.GetByIdAsync(id);

        if (existingPlace == null)
            throw new ArgumentException("Place with the provided Id does not exist.");

        place.MapUpsertPlaceDtoToPlace(existingPlace);

        await repository.UpdateAsync(existingPlace);
    }

    public async Task DeletePlace(int id)
    {
        var place = await repository.GetByIdAsync(id);

        if (place == null)
            throw new ArgumentException("Place with provided Id does not exist");

        if (place.PlantedList != null && place.PlantedList.Any())
            throw new ArgumentException("You have to move planted plants to another place before deleting");

        await repository.DeleteAsync(place);
    }
}
