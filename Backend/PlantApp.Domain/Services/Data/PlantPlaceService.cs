using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security.Authentication;

namespace PlantApp.Domain.Services.Data;

public class PlantPlaceService(
    IRepository<Place> repository,
    IRepository<Country> countryRepository,
    ICurrentUserContext userContext,
    ILogger<PlantPlaceService> logger
) : IPlantPlaceService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    public async Task<List<PlaceDto>> GetAllAsync(int userId)
    {
        var places = await repository.GetAllByKeyAsync(p => p.UserId == userId, true);
        var sortedPlaces = places.OrderBy(p => p.Name)
            .ThenBy(p => p.Country != null ? p.Country.Name : p.City)
            .ThenBy(p => p.City)
            .ThenBy(p => p.Address)
            .ToList();

        logger.LogInformation("Retrieved {Count} places for user {UserId}", sortedPlaces.Count, userId);

        return sortedPlaces.Select(p => p.MapPlaceToPlaceDto()).ToList();
    }

    public async Task<PlaceGetDto> GetByIdAsync(int id)
    {
        var place = await repository.GetByIdAsync(id);

        if (place == null)
        {
            logger.LogWarning("Place {PlaceId} not found", id);
            throw new KeyNotFoundException("The requested place does not exist.");
        }

        if (place.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to access place {PlaceId} without permission", CurrentUserId, id);
            throw new AuthenticationException("You are not authorized to access this place.");
        }

        return place.MapPlaceToPlaceGetDto();
    }

    public async Task AddAsync(UpsertPlaceDto dto)
    {
        var country = countryRepository.GetByIdAsync(dto.CountryId);
        if (country == null)
        {
            logger.LogWarning("Country {CountryId} not found when adding place", dto.CountryId);
            throw new KeyNotFoundException("The selected country does not exist.");
        }

        var place = dto.MapUpsertPlaceDtoToPlace();
        
        place.UserId = CurrentUserId;
        await repository.AddAsync(place);

        logger.LogInformation("Place {PlaceId} added by user {UserId}", place.Id, CurrentUserId);
    }

    public async Task UpdateAsync(int id, UpsertPlaceDto dto)
    {
        if (id != dto.Id)
        {
            logger.LogWarning("DTO ID {DtoId} does not match route ID {Id}", dto.Id, id);
            throw new ArgumentException("DTO ID does not match the provided route ID.");
        }

        var existingPlace = await repository.GetByIdAsync(id);

        if (existingPlace == null)
        {
            logger.LogWarning("Place {PlaceId} not found for update", id);
            throw new KeyNotFoundException("The place you are trying to update does not exist.");
        }

        if (existingPlace.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to update place {PlaceId} without permission", CurrentUserId, id);
            throw new AuthenticationException("You are not authorized to update this place.");
        }

        dto.MapUpsertPlaceDtoToPlace(existingPlace);
        await repository.UpdateAsync(existingPlace);

        logger.LogInformation("Place {PlaceId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var place = await repository.GetByIdAsync(id);

        if (place == null)
        {
            logger.LogWarning("Place {PlaceId} not found for deletion", id);
            throw new KeyNotFoundException("The place you are trying to delete does not exist.");
        }

        if (place.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to delete place {PlaceId} without permission", CurrentUserId, id);
            throw new AuthenticationException("You are not authorized to delete this place.");
        }

        if (place.PlantedList != null && place.PlantedList.Any())
        {
            logger.LogWarning("Place {PlaceId} contains planted plants and cannot be deleted", id);
            throw new InvalidOperationException(
                "You must remove or move planted plants before deleting this place.");
        }

        await repository.DeleteAsync(place, false);
        logger.LogInformation("Place {PlaceId} deleted by user {UserId}", id, CurrentUserId);
    }
}
