using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class PlantPlaceService(
    IRepository<Place> repository,
    IRepository<Country> countryRepository,
    ICurrentUserContext userContext,
    ILogger<PlantPlaceService> logger
) : IPlantPlaceService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<List<PlaceDto>> GetAllAsync()
    {
        int userId = CurrentUserId;
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
            throw new NotFoundException("Place", id, logger);
        if (place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("access", "place", logger);

        return place.MapPlaceToPlaceGetDto();
    }

    public async Task AddAsync(UpsertPlaceDto dto)
    {
        var country = countryRepository.GetByIdAsync(dto.CountryId);
        if (country == null) 
            throw new NotFoundException("Country", dto.CountryId, logger);

        var place = dto.MapUpsertPlaceDtoToPlace();
        place.UserId = CurrentUserId;

        await repository.AddAsync(place);

        logger.LogInformation("Place {PlaceId} added by user {UserId}", place.Id, CurrentUserId);
    }

    public async Task UpdateAsync(int id, UpsertPlaceDto dto)
    {
        if (id != dto.Id)
        {
            throw new DtoIdMismatchException("Place", dto.Id ?? 0, id, logger);
        }

        var existingPlace = await repository.GetByIdAsync(id);

        if (existingPlace == null) 
            throw new NotFoundException("Place", id, logger);
        if (existingPlace.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "place", logger);

        dto.MapUpsertPlaceDtoToPlace(existingPlace);
        await repository.UpdateAsync(existingPlace);

        logger.LogInformation("Place {PlaceId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var place = await repository.GetByIdAsync(id);

        if (place == null) 
            throw new NotFoundException("Place", id, logger);
        if (place.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("delete", "place", logger);

        if (place.PlantedList != null && place.PlantedList.Any())
        {
            throw new InvalidOperationAppException(
                userMessage: "This place cannot be deleted while it contains plants.",
                internalMessage: $"Place {id} has planted items and delete was attempted.",
                logger: logger
            );
        }

        await repository.DeleteAsync(place, false);
        logger.LogInformation("Place {PlaceId} deleted by user {UserId}", id, CurrentUserId);
    }
}
