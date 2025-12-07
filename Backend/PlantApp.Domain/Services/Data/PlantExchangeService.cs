using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security.Authentication;

namespace PlantApp.Domain.Services.Data;

public class PlantExchangeService(
    IRepository<PlantExchange> repository,   
    IRepository<Country> countryRepo,
    IRepository<Planted> plantedRepo
) : IPlantExchangeService
{
    //user rating

    public int currentUser = 0;
    public async Task<List<PlantExchangeDto>> GetActivePlantExchanges()
    {
        var exchanges = await repository.GetAllByKeyAsync(e => e.IsActive == true);
        
        exchanges = exchanges.OrderByDescending(e => e.CreatedAt).ToList();

        return exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();
    }

    public async Task<List<PlantExchangeDto>> GetActivePlantsFiltered(PlantExchangeFilterDto filter)
    {
        var exchanges = await repository.GetAllByKeyAsync(e =>
            e.IsActive == true &&
            (string.IsNullOrWhiteSpace(filter.Name) ||
                EF.Functions.ILike(e.Title, $"%{filter.Name}%") ||
                EF.Functions.ILike(e.Content, $"%{filter.Name}%")) &&
            (string.IsNullOrWhiteSpace(filter.City) || EF.Functions.ILike(e.City, $"%{filter.City}%")) &&
            (filter.PriceFrom == null || e.Price == null || e.Price > filter.PriceFrom) &&
            (filter.PriceTo == null || e.Price == null || e.Price < filter.PriceTo) &&
            (filter.ExchangeType == null || e.ExchangeTypeId == filter.ExchangeType));

        exchanges = exchanges.OrderByDescending(e => e.CreatedAt).ToList();

        return exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();
    }

    public async Task<PlantExchangeGetDto> GetPlantExchange(int id)
    {
        var exchange = await repository.GetByIdAsync(id);

        if (exchange == null)
            throw new ArgumentException("Plant exchange not found");      

        return exchange.MapPlantExchangeToPlantExchangeGetDto();
    }

    public async Task AddPlantExchange(UpsertPlantExchangeDto dto)
    {
        dto.PlantedId = await ValidatePlantExchange(dto);

        var exchange = dto.MapUpsertPlantExchangeDtoToPlantExchange();

        await repository.AddAsync(exchange);
    }

    public async Task UpdatePlantExchange(int id, UpsertPlantExchangeDto dto)
    {

        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null)
            throw new ArgumentException("Plant Exchange not found");

        if (existingExchange.UserId != currentUser)
            throw new AuthenticationException("Access denied");

        dto.PlantedId = await ValidatePlantExchange(dto);

        dto.MapUpsertPlantExchangeDtoToPlantExchange(existingExchange);

        await repository.UpdateAsync(existingExchange);
    }

    public async Task DeletePlantExchange(int id)
    {
        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null)
            throw new ArgumentException("Plant Exchange not found");

        if (existingExchange.UserId != currentUser)
            throw new AuthenticationException("Access denied");

        await repository.DeleteAsync(existingExchange, false);
    }

    public async Task<int?> ValidatePlantExchange(UpsertPlantExchangeDto dto)
    {
        if (dto.PlantedId != null)
        {
            var planted = await plantedRepo.GetByIdAsync(dto.PlantedId.Value);

            if (planted == null)
                dto.PlantedId = null;

            if (planted.Place.UserId != currentUser)
                throw new AuthenticationException("Cannot add reference to the planted");
        }

        var countryExists = await countryRepo.IdExistsAsync(dto.CountryId);
        if (!countryExists)
        {
            throw new ArgumentException("Invalid country");
        }

        return dto.PlantedId;
    }
}
