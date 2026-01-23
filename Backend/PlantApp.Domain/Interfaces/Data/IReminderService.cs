using PlantApp.Domain.Dtos.Reminder;

namespace PlantApp.Domain.Interfaces.Data;

public interface IReminderService
{
    public Task<List<ReminderDto>> GetAllAsync();
    public Task<ReminderGetDto> GetByIdAsync(int id);
    public Task ReminderDoneAsync(int id, DateTime? dateDone);
    public Task DelayReminderAsync(int id, int delay);
    public Task AddAsync(UpsertReminderDto dto);
    public Task UpdateAsync(int id, UpsertReminderDto dto);
    public Task DeleteAsync(int id);
    public Task<ReminderReferences> GetReferences();
}
