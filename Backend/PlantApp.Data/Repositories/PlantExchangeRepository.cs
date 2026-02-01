using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class PlantExchangeRepository(AppDbContext context) : Repository<PlantExchange>(context), IPlantExchangeRepository 
{
    private const int pageSize = 25;
    public async Task<(int, List<PlantExchange>)> GetActivePlantExchanges(int page)
    {
        var query = dbSet.Where(q => q.IsActive && q.DeletedAt == null);
        var total = await query.CountAsync();

        var sortedQuery = query.OrderByDescending(q => q.CreatedAt);
        var projectedQuery = ProjectPlantExchangeForList(sortedQuery);

        return (total, await projectedQuery
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync());
    }

    public async Task<(int, List<PlantExchange>)> GetPlantExchangesFiltered(PlantExchangeFilterDto filter, int page)
    {
        var query = dbSet.Where(e =>
            e.IsActive == true &&
            e.DeletedAt == null &&
            (string.IsNullOrWhiteSpace(filter.Name) ||
                EF.Functions.ILike(e.Title, $"%{filter.Name}%") ||
                EF.Functions.ILike(e.Content, $"%{filter.Name}%")) &&
            (string.IsNullOrWhiteSpace(filter.City) ||
                EF.Functions.ILike(e.City, $"%{filter.City}%") ||
                (e.Country != null && EF.Functions.ILike(e.Country.Name, $"%{filter.City}%"))) &&
            (filter.PriceFrom == null || e.Price == null || e.Price > filter.PriceFrom) &&
            (filter.PriceTo == null || e.Price == null || e.Price < filter.PriceTo) &&
            (filter.ExchangeType == null || e.ExchangeTypeId == filter.ExchangeType)
        );

        var total = await query.CountAsync();
        var sortedQuery = query.OrderByDescending(q => q.CreatedAt);
        var projectedQuery = ProjectPlantExchangeForList(sortedQuery);

        return (total, await projectedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync());
    }

    private IQueryable<PlantExchange> ProjectPlantExchangeForList(IQueryable<PlantExchange> query)
    {
        return query.Select(q => new PlantExchange
        {
            Id = q.Id,
            User = q.User,
            Title = q.Title,
            ExchangeTypeId = q.ExchangeTypeId,
            ExchangeType = q.ExchangeType,
            Shipping = q.Shipping,
            MainImage = q.MainImage,
            CountryId = q.CountryId,
            Country = q.Country,
            Price = q.Price,
            PlantStatus = q.PlantStatus,
            Content = q.Content,
            Contact = q.Contact,
            City = q.City,
            CreatedAt = q.CreatedAt,
        });
    }

    public async Task<PlantExchange?> GetPlantExchangeById(int id)
    {
        var query = dbSet.AsQueryable();
        query = IncludeNavigations(query);
        query = query
            .Include(q => q.User)
                .ThenInclude(u => u.RatingsReceived)
                    .ThenInclude(r => r.Rater);

        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }
}
