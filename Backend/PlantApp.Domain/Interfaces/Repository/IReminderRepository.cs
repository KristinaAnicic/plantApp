using PlantApp.Domain.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IReminderRepository : IRepository<Reminder>
{
    public Task<List<Reminder>> GetAllRemindersAsync(int userId);
    public Task<Reminder?> GetReminderAsync(int id);
}
