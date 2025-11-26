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

    public async Task<Dictionary<Place, List<Planted>>> GetPlantedPlantsByUserIdGrouped(int userId, bool filterByName = false)
    {
        var query = FilterPlanted(userId)
            .Include(p => p.Reminders)
            .AsQueryable();

        query = OrderPlanted(query, filterByName);

        return await query
        .GroupBy(p => new { p.PlaceId, p.Place })
        .ToDictionaryAsync(
            g => g.Key.Place!,
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
            .Include(p => p.Images)
            .Include(p => p.PlantStatus)
            .Where(p => p.Plant != null && p.Place != null && p.Place.UserId == userId && p.PlantStatusId != 3);
    }
    private IQueryable<Planted> OrderPlanted(IQueryable<Planted> query, bool filterByName)
    {
        if (filterByName)
        {
            query = query.OrderBy(p => p.Plant!.CommonName);
        }
        else
        {
            query = query.OrderBy(p => p.Reminders
                                        .Select(r => (r.NextDueDate.AddDays(r.DelayDays) - DateTime.UtcNow).TotalDays)
                                        .DefaultIfEmpty(double.MaxValue)
                                        .Min())
                .ThenBy(p => p.UpdatedAt)
                .ThenBy(p => p.CreatedAt);
        }

        return query;
    }
}
