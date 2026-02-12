using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class ReminderRepository(AppDbContext context) : Repository<Reminder>(context), IReminderRepository
{
    public async Task<List<Reminder>> GetAllRemindersAsync(int userId)
    {
        var query = dbSet
            .Include(q => q.Planted)
                .ThenInclude(p => p.Place)
            .Include(r => r.Planted)
                .ThenInclude(p => p.Plant)
            .Include(r => r.ReminderType)
            .Where(q => q.Planted != null && q.Planted.Place != null && q.Planted.Place.UserId == userId)
            .OrderBy(q => q.NextDueDate);

        return await query.ToListAsync();
    }

    public async Task<List<Reminder>> GetPendingRemindersAsync(int userId)
    {
        var query = dbSet
            .Include(q => q.Planted)
                .ThenInclude(p => p.Place)
            .Include(r => r.Planted)
                .ThenInclude(p => p.Plant)
            .Include(r => r.ReminderType)
            .Where(q => q.Planted != null && 
                q.Planted.Place != null && 
                q.Planted.Place.UserId == userId && 
                q.NextDueDate.Date <= DateTime.UtcNow.Date)
            .OrderBy(q => q.NextDueDate);

        return await query.ToListAsync();
    }

    public async Task<List<Reminder>> GetAllRemindersByPlantGroupId(int plantGroupId)
    {
        var query = dbSet
            .Include(q => q.Planted)
                .ThenInclude(p => p.Place)
            .Include(r => r.Planted)
                .ThenInclude(p => p.Plant)
            .Include(r => r.ReminderType)
            .Include(r => r.FrequencyType)
            .Where(q => q.DeletedAt == null && 
                        q.Planted != null && 
                        q.Planted.PlantGroupId == plantGroupId)
            .OrderBy(q => q.NextDueDate)
            .ThenBy(q => q.CreatedAt);

        return await query.ToListAsync();
    }

    public async Task<Reminder?> GetReminderAsync(int id)
    {
        var query = dbSet
            .Include(q => q.Planted)
                .ThenInclude(p => p.Place)
            .Include(r => r.Planted)
                .ThenInclude(p => p.Plant)
            .Include(r => r.ReminderType)
            .Include(r => r.FrequencyType);

        return await query.FirstOrDefaultAsync(q => q.Id == id);
    }
}
