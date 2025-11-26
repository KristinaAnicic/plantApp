using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Domain.Repositories;

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
            .OrderBy(q => (q.NextDueDate.AddDays(q.DelayDays) - DateTime.UtcNow).TotalDays);

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

        return await query.FirstOrDefaultAsync();
    }
}
