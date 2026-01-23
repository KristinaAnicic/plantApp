using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Repositories;

public class GrowthLogRepository(AppDbContext context) : Repository<GrowthLog>(context), IGrowthLogRepository
{
    public async Task<List<GrowthLog>> GetAllGrowthLogsByUserId(int userId)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        query = query
            .Where(q => q.Planted != null && 
                        q.Planted.Place != null && 
                        q.Planted.Place.UserId == userId)
            .OrderByDescending(q => q.ObservationDate)
            .ThenByDescending(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<List<GrowthLog>> GetAllGrowthLogsByPlantedId(int plantedId)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        query = query
            .Where(q => q.PlantedId == plantedId)
            .OrderByDescending(q => q.ObservationDate)
            .ThenByDescending(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<GrowthLog?> GetGrowthLogById(int id)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task DeleteGrowthLog(GrowthLog log)
    {
        var imageIds = log.Images.Select(x => x.Id).ToList();

        var images = await context.Images
            .Where(i => imageIds.Contains(i.Id))
            .ToListAsync();

        context.Images.RemoveRange(images);
        dbSet.Remove(log);

        await context.SaveChangesAsync();
    }

    private IQueryable<GrowthLog> AddIncludes(IQueryable<GrowthLog> query)
    {
        query = query
            .Include(q => q.Planted)
                .ThenInclude(p => p.Place)
            .AsQueryable();

        query = IncludeNavigations(query);

        return query;
    }
}
