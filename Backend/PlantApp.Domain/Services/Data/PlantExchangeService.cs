using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security.Authentication;

namespace PlantApp.Domain.Services.Data;

public class PlantExchangeService(
    IRepository<PlantExchange> repository,   
    IRepository<Country> countryRepo,
    IRepository<Planted> plantedRepo,
    IImageService imageService
) : IPlantExchangeService
{
    //user rating

    public int currentUser = 3;
    public async Task<ListResponse<PlantExchangeDto>> GetActiveAsync(int page = 1)
    {
        var exchanges = await repository.GetAllByKeyAsync(e => e.IsActive == true, false, page);
        
        exchanges = exchanges.OrderByDescending(e => e.CreatedAt).ToList();
        var dto = exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();
        var total = await repository.CountAsync();

        return new ListResponse<PlantExchangeDto> { Total = total, Items = dto };
    }

    public async Task<ListResponse<PlantExchangeDto>> GetActiveFilteredAsync(PlantExchangeFilterDto filter, int page = 1)
    {
        var exchanges = await repository.GetAllByKeyAsync(
            e =>
                e.IsActive == true &&
                (string.IsNullOrWhiteSpace(filter.Name) ||
                    EF.Functions.ILike(e.Title, $"%{filter.Name}%") ||
                    EF.Functions.ILike(e.Content, $"%{filter.Name}%")) &&
                (string.IsNullOrWhiteSpace(filter.City) || EF.Functions.ILike(e.City, $"%{filter.City}%")) &&
                (filter.PriceFrom == null || e.Price == null || e.Price > filter.PriceFrom) &&
                (filter.PriceTo == null || e.Price == null || e.Price < filter.PriceTo) &&
                (filter.ExchangeType == null || e.ExchangeTypeId == filter.ExchangeType),   
            false, page);

        exchanges = exchanges.OrderByDescending(e => e.CreatedAt).ToList();

        var dto = exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();
        var total = await repository.CountAsync();

        return new ListResponse<PlantExchangeDto> { Total = total, Items = dto };
    }

    public async Task<PlantExchangeGetDto> GetByIdAsync(int id)
    {
        var exchange = await repository.GetByIdAsync(id);

        if (exchange == null)
            throw new ArgumentException("Plant exchange not found");      

        return exchange.MapPlantExchangeToPlantExchangeGetDto();
    }

    public async Task AddAsync(UpsertPlantExchangeDto dto)
    {
        dto.PlantedId = await ValidatePlantExchange(dto);

        var exchange = dto.MapUpsertPlantExchangeDtoToPlantExchange();

        if (dto.Images != null && dto.Images.Any())
        {
            exchange.Images.Clear();
            await imageService.AddImagesSafeAsync(exchange, dto.Images);
        }

        await repository.AddAsync(exchange);
    }

    public async Task UpdateAsync(int id, UpsertPlantExchangeDto dto)
    {

        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null)
            throw new ArgumentException("Plant Exchange not found");

        if (existingExchange.UserId != currentUser)
            throw new AuthenticationException("Access denied");

        dto.PlantedId = await ValidatePlantExchange(dto);

        dto.MapUpsertPlantExchangeDtoToPlantExchange(existingExchange);

        if (dto.Images != null && dto.Images.Any())
        {
            existingExchange.Images.Clear();

            await imageService.AddImagesSafeAsync(existingExchange, dto.Images);
        }

        await repository.UpdateAsync(existingExchange);
    }

    public async Task DeleteAsync(int id)
    {
        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null)
            throw new ArgumentException("Plant Exchange not found");

        if (existingExchange.UserId != currentUser)
            throw new AuthenticationException("Access denied");

        await repository.DeleteAsync(existingExchange, false);
    }

    public async Task AddImages(int exchangeId, List<string> urls)
    {
        var exchange = await repository.GetByIdAsync(exchangeId);
        if (exchange == null)
            throw new ArgumentException("Plant not found");

        if (exchange.UserId != currentUser)
            throw new InvalidOperationException("Access denied");

        await imageService.AddImagesToEntityAsync(exchange, urls);
        await repository.UpdateAsync(exchange);
    }

    public async Task<string?> RemoveImageById(int exchangeId, int imageId)
    {
        var exchange = await repository.GetByIdAsync(exchangeId);
        if (exchange == null)
            throw new ArgumentException("Plant not found");

        var deletedUrl = await imageService.RemoveImageFromEntityAsync(exchange, imageId, repository);
        //await repository.UpdateAsync(exchange);

        return deletedUrl;
    }

    public async Task<int?> ValidatePlantExchange(UpsertPlantExchangeDto dto)
    {
        if (dto.PlantedId != null)
        {
            var planted = await plantedRepo.GetByIdAsync(dto.PlantedId.Value);

            if (planted == null)
                dto.PlantedId = null;

            if (planted != null && planted.Place != null && planted.Place.UserId != currentUser)
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
