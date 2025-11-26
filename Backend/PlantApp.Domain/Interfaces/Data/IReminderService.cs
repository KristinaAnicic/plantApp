using PlantApp.Domain.Dtos.Reminder;

namespace PlantApp.Domain.Interfaces.Data;

public interface IReminderService
{
    public Task<List<ReminderDto>> GetReminders();
    public Task<ReminderGetDto> GetReminder(int id);
    public Task ReminderDone(int id, DateTime? dateDone);
    public Task DelayReminder(int id, int delay);
    public Task AddReminder(UpsertReminderDto dto);
    public Task UpdateReminder(int id, UpsertReminderDto dto);
    public Task DeleteReminder(int id);
}
