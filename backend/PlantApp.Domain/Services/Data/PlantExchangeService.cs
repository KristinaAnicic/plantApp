using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
using PlantApp.Domain.Models.Interfaces;

namespace PlantApp.Domain.Services.Data;

public class PlantExchangeService(
    IPlantExchangeRepository repository,   
    IRepository<Country> countryRepo,
    IPlantedRepository plantedRepo,
    IRepository<ExchangeType> exchangeTypeRepo,
    IImageService imageService,
    ICurrentUserContext userContext,
    ILogger<PlantExchangeService> logger
) : IPlantExchangeService
{
    //user rating

    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<ListResponse<PlantExchangeDto>> GetActiveAsync(int page = 1)
    {
        var (total, exchanges) = await repository.GetActivePlantExchanges(page);        
        var dto = exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();

        return new ListResponse<PlantExchangeDto> { Total = total, Items = dto };
    }

    public async Task<ListResponse<PlantExchangeDto>> GetActiveFilteredAsync(PlantExchangeFilterDto filter, int page = 1)
    {
        var (total, exchanges) = await repository.GetPlantExchangesFiltered(filter, page);
        var dto = exchanges.Select(e => e.MapPlantExchangeToPlantExchangeDto()).ToList();

        return new ListResponse<PlantExchangeDto> { Total = total, Items = dto };
    }

    public async Task<PlantExchangeGetDto> GetByIdAsync(int id)
    {
        var exchange = await repository.GetPlantExchangeById(id);

        if (exchange == null) 
            throw new NotFoundException("Plant exchange", id, logger);

        var dto = exchange.MapPlantExchangeToPlantExchangeGetDto();
        var userId = userContext.TryGetCurrentUserId();

        if (dto.UserRatings != null && dto.UserRatings.Any())
        {
            dto.UserRatings = dto.UserRatings
                .OrderByDescending(r => userId != null && r.Rater.Id == userId)
                .ThenByDescending(r => r.CreatedAt)
                .ToList();
        }

        return dto;
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
        exchange.UserId = CurrentUserId;
        await repository.AddAsync(exchange);

        logger.LogInformation("Plant exchange added by user {UserId}", CurrentUserId);
    }

    public async Task UpdateAsync(int id, UpsertPlantExchangeDto dto)
    {
        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null) 
            throw new NotFoundException("Plant exchange", id, logger);
        if (existingExchange.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "plant exchange", logger);

        dto.PlantedId = await ValidatePlantExchange(dto);
        dto.MapUpsertPlantExchangeDtoToPlantExchange(existingExchange);

        if (dto.Images != null && dto.Images.Any())
        {
            existingExchange.Images.Clear();
            await imageService.AddImagesSafeAsync(existingExchange, dto.Images);
        }

        await repository.UpdateAsync(existingExchange);

        await imageService.RemoveUnusedImagesAsync();

        logger.LogInformation("Plant exchange {ExchangeId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var existingExchange = await repository.GetByIdAsync(id);

        if (existingExchange == null) 
            throw new NotFoundException("Plant exchange", id, logger);
        if (existingExchange.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("delete", "plant exchange", logger);

        await repository.DeleteAsync(existingExchange, false);
        logger.LogInformation("Plant exchange {ExchangeId} deleted by user {UserId}", id, CurrentUserId);
    }

    public async Task AddImages(int exchangeId, List<string> urls)
    {
        var exchange = await repository.GetByIdAsync(exchangeId);

        if (exchange == null) 
            throw new NotFoundException("Plant exchange", exchangeId, logger);
        if (exchange.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("add images to", "plant exchange", logger);

        await imageService.AddImagesToEntityAsync(exchange, urls);
        await repository.UpdateAsync(exchange);
    }

    public async Task RemoveImageById(int exchangeId, int imageId)
    {
        var exchange = await repository.GetByIdAsync(exchangeId);
        if (exchange == null) 
            throw new NotFoundException("Plant exchange", exchangeId, logger);
        if (exchange.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("remove images from", "plant exchange", logger);

        await imageService.RemoveImageFromEntityAsync(exchange, imageId, repository);
    }

    public async Task<PlantExchangeReferences> GetReferences()
    {
        var exchangeTypes = await exchangeTypeRepo.GetAllAsync();
        var planted = await plantedRepo.GetPlantedPlantsByUserId(CurrentUserId);

        return new PlantExchangeReferences
        {
            Planted = planted.Select(p => new ReferenceDto
            {
                Id = p.Id,
                Name = p.Name ?? (p.Plant != null 
                                    ? (p.Plant.CommonName ?? p.Plant.BotanicalName) 
                                    : "Unknown")
            }).ToList(),
            ExchangeTypes = exchangeTypes.Select(t => t.MapReferenceToDto()).ToList()
        };
    }

    public async Task<int?> ValidatePlantExchange(UpsertPlantExchangeDto dto)
    {
        if (dto.PlantedId != null)
        {
            var planted = await plantedRepo.GetByIdAsync(dto.PlantedId.Value);

            if (planted == null)
            {
                logger.LogWarning("Invalid planted reference {PlantedId}", dto.PlantedId);
                return null;
            }

            if (planted != null && planted.Place != null && planted.Place.UserId != CurrentUserId && !IsAdmin)
            {
                throw new UnauthorizedException("use planted plant in", "plant exchange", logger);
            }
        }

        if (!await countryRepo.IdExistsAsync(dto.CountryId)) throw new NotFoundException("Country", dto.CountryId, logger);

        return dto.PlantedId;
    }
}
