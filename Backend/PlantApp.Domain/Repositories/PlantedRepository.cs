using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Repositories;

public class PlantedRepository(AppDbContext context) : Repository<Planted>(context), IPlantedRepository
{
    public async Task<List<Planted>> GetPlantedPlantsByUserId(int userId, bool filterByName = false)
    {
        var query = FilterPlanted(userId);
        query = OrderPlanted(query, filterByName);

        return await query.ToListAsync();
    }

    public async Task<List<Planted>> GetPlantedPlantsByPlaceId(int placeId)
    {
        var query = dbSet
            .Include(p => p.Place)
            .Include(p => p.Plant)
                .ThenInclude(p => p.Images)
            .Include(p => p.Images)
            .Include(p => p.PlantStatus)
            .Where(p => p.Plant != null && p.Place != null && p.Place.Id == placeId && p.PlantStatusId != 3)
            .OrderBy(x => x.UpdatedAt);

        return await query.ToListAsync();
    }

    public async Task<Planted?> GetPlantedById(int id)
    {
        var query = dbSet.AsQueryable();
        query = IncludeNavigations(query);
        query = query
            .Include(q => q.Place!.Country)
            .Include(q => q.Plant!.Images)
            .Include(q => q.Reminders)
                .ThenInclude(r => r.ReminderType);

        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Dictionary<Place, List<Planted>>> GetPlantedPlantsByUserIdGrouped(int userId, bool filterByName = false)
    {
        var query = FilterPlanted(userId)
            .Include(p => p.Reminders)
            .Include(p => p.Place!.Country)
            .AsQueryable();

        query = OrderPlanted(query, filterByName);

        return await query
        .GroupBy(p => p.Place)
        .ToDictionaryAsync(
            g => g.Key!,
            g => g.ToList());
    }

    public async Task DeletePlantedAsync(Planted planted)
    {
        var reminders = context.Reminders.Where(r => r.PlantedId == planted.Id);
        var growthLogs = context.GrowthLogs.Where(gl => gl.PlantedId == planted.Id);

        context.Reminders.RemoveRange(reminders);
        context.GrowthLogs.RemoveRange(growthLogs);
        dbSet.Remove(planted);
        await context.SaveChangesAsync();
    }

    private IQueryable<Planted> FilterPlanted(int userId)
    {
        return dbSet
            .Include(p => p.Place)
            .Include(p => p.Plant)
                .ThenInclude(p => p.Images)
            .Include(p => p.Images)
            .Include(p => p.PlantStatus)
            .Where(p => p.Plant != null && p.Place != null && p.Place.UserId == userId && p.PlantStatusId != 3);
    }
    /*private IQueryable<Planted> OrderPlanted(IQueryable<Planted> query, bool filterByName)
    {
        if (filterByName)
        {
            query = query.OrderBy(p => p.Plant!.CommonName);
        }
        else
        {
            query = query.OrderBy(p => p.Reminders
                                        .Select(r => r.NextDueDate.AddDays(r.DelayDays))
                                        .DefaultIfEmpty(DateTime.MaxValue)
                                        .Min())
                .ThenBy(p => p.UpdatedAt)
                .ThenBy(p => p.CreatedAt);
        }

        return query;
    }*/

    private IQueryable<Planted> OrderPlanted(IQueryable<Planted> query, bool filterByName)
    {
        if (filterByName)
        {
            return query.OrderBy(p => p.Plant!.CommonName);
        }
        else
        {
            var queryWithMin = query
                .Select(p => new
                {
                    Planted = p,
                    MinReminderDate = p.Reminders
                        .OrderBy(r => r.NextDueDate.AddDays(r.DelayDays))
                        .Select(r => (DateTime?)r.NextDueDate.AddDays(r.DelayDays))
                        .FirstOrDefault() ?? DateTime.MaxValue
                })
                .OrderBy(x => x.MinReminderDate)
                .ThenBy(x => x.Planted.UpdatedAt)
                .ThenBy(x => x.Planted.CreatedAt)
                .Select(x => x.Planted);

            return queryWithMin;
        }
    }
}
