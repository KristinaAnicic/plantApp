using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class PlantedRepository(AppDbContext context) : Repository<Planted>(context), IPlantedRepository
{
    public async Task<List<Planted>> GetPlantedPlantsByUserId(int userId, bool filterByName = false)
    {

        var query = dbSet
            .Where(p => p.PlantStatusId != 3 && p.Place != null && p.Place.UserId == userId && p.DeletedAt == null)
            .Select(p => new
            {
                Planted = p,
                LastActivity = p.GrowthLogs.Max(q => (DateTime?)q.CreatedAt) ?? p.UpdatedAt
            })
            .OrderByDescending(p => p.LastActivity)
            .ThenByDescending(p => p.Planted.CreatedAt)
            .Select(p => p.Planted);

        return await ProjectPlantedForList(query).ToListAsync();
    }

    public async Task<int> GetNumOfDeadPlants(int userId)
    {

        var query = dbSet
            .Where(p => p.PlantStatusId == 3 && p.Place != null && p.Place.UserId == userId && p.DeletedAt == null);

        return await query.CountAsync();
    }

    public async Task<List<Planted>> GetPlantedPlantsByPlaceId(int placeId)
    {
        var query = dbSet
            .Where(p => p.PlaceId == placeId && p.PlantStatusId != 3)
            .Select(p => new
            {
                Planted = p,
                LastActivity = p.GrowthLogs.Max(q => (DateTime?)q.CreatedAt) ?? p.UpdatedAt
            })
            .OrderByDescending(p => p.LastActivity)
            .ThenByDescending(p => p.Planted.CreatedAt)
            .Select(p => p.Planted);

        return await ProjectPlantedForList(query).ToListAsync();
    }

    public async Task<List<Planted>> GetAllDeadPlantsAsync(int userId)
    {

        var query = dbSet
            .Where(p => p.PlantStatusId == 3 && p.Place != null && p.Place.UserId == userId && p.DeletedAt == null)
            .Select(p => new
            {
                Planted = p,
                LastActivity = p.GrowthLogs.Max(q => (DateTime?)q.CreatedAt) ?? p.UpdatedAt
            })
            .OrderByDescending(p => p.LastActivity)
            .ThenByDescending(p => p.Planted.UpdatedAt)
            .ThenByDescending(p => p.Planted.CreatedAt)
            .Select(p => p.Planted);

        return await ProjectPlantedForList(query).ToListAsync();
    }

    private IQueryable<Planted> ProjectPlantedForList(IQueryable<Planted> query)
    {
        return query.Select(q => new Planted
        {
            Id = q.Id,
            PlaceId = q.PlaceId,
            Place = q.Place,
            PlantId = q.PlantId,
            Name = q.Name ?? (q.Plant != null ? q.Plant.CommonName ?? q.Plant.BotanicalName : null),
            DatePlanted = q.DatePlanted,
            IsOutside = q.IsOutside,
            Image = q.Image ??
                    (q.Plant != null && q.Plant.Images.Any() ? q.Plant.Images.Select(i => i.Url).FirstOrDefault() :
                    q.Images.Any() ? q.Images.Select(i => i.Url).FirstOrDefault() : null),
            PlantStatus = q.PlantStatus,
            PlantStatusId = q.PlantStatusId,
            CreatedAt = q.CreatedAt
        });
    }

    public async Task<Planted?> GetPlantedById(int id)
    {
        var query = dbSet.AsQueryable();
        query = IncludeNavigations(query);
        query = query
            .Include(q => q.Place!.Country)
            .Include(q => q.Plant!.Images)
            .Include(q => q.Reminders)
                .ThenInclude(r => r.ReminderType)
            .Include(q => q.Reminders)
                .ThenInclude(r => r.FrequencyType)
            .Include(q => q.GrowthLogs)
                .ThenInclude(g => g.Images)
            .Include(q => q.GrowthLogs)
                .ThenInclude(g => g.PlantStatus)
            .Where(p => p.DeletedAt == null);

        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Dictionary<Place, List<Planted>>> GetPlantedPlantsByUserIdGrouped(int userId, bool filterByName = false)
    {
        var query = FilterPlantedByUserId(userId)
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

    private IQueryable<Planted> FilterPlantedByUserId(int userId)
    {
        return dbSet
            .Include(p => p.Place)
            .Include(p => p.Plant)
                .ThenInclude(p => p.Images)
            .Include(p => p.Images)
            .Include(p => p.PlantStatus)
            .Where(p => p.Plant != null && p.Place != null && p.Place.UserId == userId && p.PlantStatusId != 3 && p.DeletedAt == null);
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

    private IQueryable<Planted> OrderPlanted(IQueryable<Planted> query, bool orderByName)
    {
        if (orderByName)
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
                        .OrderBy(r => r.NextDueDate)
                        .Select(r => (DateTime?)r.NextDueDate)
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
