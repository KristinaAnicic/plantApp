using PlantApp.Domain.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IReminderRepository : IRepository<Reminder>
{
    public Task<List<Reminder>> GetAllRemindersAsync(int userId);
    public Task<List<Reminder>> GetPendingRemindersAsync(int userId);
    public Task<List<Reminder>> GetAllRemindersByPlantGroupId(int plantGroupId);
    public Task<Reminder?> GetReminderAsync(int id);
}
