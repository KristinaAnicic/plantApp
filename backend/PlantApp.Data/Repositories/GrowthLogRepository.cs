using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class GrowthLogRepository(AppDbContext context) : Repository<GrowthLog>(context), IGrowthLogRepository
{
    public async Task<List<GrowthLog>> GetAllGrowthLogsByUserId(int userId)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        query = query
            .Where(q => q.DeletedAt == null &&
                        q.Planted
                       .Any(pl => pl.Place != null &&
                                   pl.Place.UserId == userId))
            .OrderByDescending(q => q.ObservationDate)
            .ThenByDescending(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<List<GrowthLog>> GetAllGrowthLogsByPlantGroupId(int plantGroupId)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        query = query
            .Where(q =>
                        q.DeletedAt == null &&
                        (
                            (q.PlantGroupId.HasValue && q.PlantGroupId.Value == plantGroupId) ||
                            q.Planted.Any(pl =>
                                pl.PlantGroupId.HasValue &&
                                pl.PlantGroupId.Value == plantGroupId)
                        ))
            .OrderByDescending(q => q.ObservationDate)
            .ThenByDescending(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<List<GrowthLog>> GetAllGrowthLogsByPlantedId(int plantedId, int? plantGroupId)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        query = query
            .Where(q => q.DeletedAt == null &&
                     (
                        q.Planted.Any(pl => pl.Id == plantedId) /*||
                        (plantGroupId.HasValue && q.PlantGroupId == plantGroupId.Value)*/
                     ))
            .OrderByDescending(q => q.ObservationDate)
            .ThenByDescending(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<GrowthLog?> GetGrowthLogById(int id)
    {
        var query = AddIncludes(dbSet.AsQueryable());
        return await query
            .Where(q => q.DeletedAt == null)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task DeleteGrowthLog(GrowthLog log)
    {
        var imageIds = log.Images.Select(x => x.Id).ToList();

        var images = await context.Images
            .Where(i => imageIds.Contains(i.Id))
            .ToListAsync();

        context.Images.RemoveRange(images);
        log.Planted.Clear();
        dbSet.Remove(log);

        await context.SaveChangesAsync();
    }

    public async Task AddWithTransactionAsync(GrowthLog log, List<Image>? images = null)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (images != null && images.Any())
            {
                log.Images.Clear();
                foreach (var img in images)
                {
                    log.Images.Add(img);
                }
            }

            dbSet.Add(log);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
