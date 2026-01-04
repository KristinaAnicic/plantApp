using PlantApp.Data.Models;
using PlantApp.Data.Models.Categories;
using PlantApp.Domain.Dtos.Reminder;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class ReminderService(
    IReminderRepository repository,
    IRepository<ReminderHistory> reminderHistoryRepo,
    IRepository<ReminderType> reminderTypeRepo,
    IRepository<Frequency> frequencyRepo,
    IRepository<Planted> plantedRepo
) : IReminderService
{
    public int currentUser = 3;
    public async Task<List<ReminderDto>> GetAllAsync()
    {
        var reminders = await repository.GetAllRemindersAsync(currentUser);
        return reminders.Select(r => r.MapReminderToReminderDto()).ToList();
    }

    public async Task<ReminderGetDto> GetByIdAsync(int id)
    {
        var reminder = await repository.GetReminderAsync(id);
        CheckReminderAndAuthorization(reminder);

        return reminder!.MapReminderToReminderGetDto();
    }

    //save to ReminderHistory
    //change NextDueDate
    //set delayDays to 0
    public async Task ReminderDoneAsync(int id, DateTime? dateDone)
    {
        var reminder = await repository.GetReminderAsync(id);
        CheckReminderAndAuthorization(reminder);

        DateTime dateDoneVar = dateDone ?? DateTime.UtcNow;

        var delayDays = Math.Max(
            0,
            (int)(dateDoneVar.Date - reminder!.NextDueDate.Date).TotalDays
        );

        var reminderHistory = new ReminderHistory
        {
            PlantedId = reminder!.PlantedId,
            ReminderTypeId = reminder.ReminderTypeId,
            FrequencyTypeId = reminder.FrequencyTypeId,
            FrequencyNum = reminder.FrequencyNum,
            DueDate = reminder.NextDueDate,
            DateDone = dateDoneVar,
            delay = delayDays
        };

        await reminderHistoryRepo.AddAsync(reminderHistory);

        var newDueDate = new DateTime();

        switch (reminder.FrequencyTypeId) {
            case 1:
                newDueDate = dateDoneVar.AddDays(reminder.FrequencyNum);
                break;
            case 2:
                newDueDate = dateDoneVar.AddDays(reminder.FrequencyNum * 7);
                break;
            case 3:
                newDueDate = dateDoneVar.AddMonths(reminder.FrequencyNum);
                break;
            case 4:
                newDueDate = dateDoneVar.AddYears(reminder.FrequencyNum);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(reminder.ReminderTypeId), "Unknown ReminderTypeId");
        }

        reminder.NextDueDate = newDueDate;
        reminder.DelayDays = 0;

        await repository.UpdateAsync(reminder);
    }

    public async Task DelayReminderAsync(int id, int delay)
    {
        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);

        reminder!.DelayDays = delay;
        reminder.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(reminder);
    }
    public async Task AddAsync(UpsertReminderDto dto)
    {
        await ValidateReferences(dto);
        var reminder = dto.MapUpsertReminderDtoToReminder();

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId);
        if (planted!.Place!.UserId != currentUser)
            throw new ArgumentException("Does not have access to the planted");

        await repository.AddAsync(reminder);
    }

    //on frontend disable changing nextDueDate or reminderHistory calculate delay by previous reminder
    public async Task UpdateAsync(int id, UpsertReminderDto dto)
    {
        if (dto.Id != id)
            throw new ArgumentException("DTO id does not match with provided id");

        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);
        await ValidateReferences(dto);

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId);
        if (planted!.Place!.UserId != currentUser)
            throw new ArgumentException("Does not have access to the planted");

        dto.MapUpsertReminderDtoToReminder(reminder);

        await repository.UpdateAsync(reminder!);
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);

        await repository.DeleteAsync(reminder!, false);
    }

    private async Task ValidateReferences(UpsertReminderDto dto)
    {
        if (!await frequencyRepo.IdExistsAsync(dto.FrequencyTypeId))
            throw new ArgumentException("Frequency type does not exist");

        if (!await reminderTypeRepo.IdExistsAsync(dto.ReminderTypeId))
            throw new ArgumentException("Reminder type does not exist");

        if (!await plantedRepo.IdExistsAsync(dto.PlantedId))
            throw new ArgumentException("Planted does not exist");
    }

    private void CheckReminderAndAuthorization(Reminder? reminder)
    {
        if (reminder == null)
            throw new ArgumentException("Reminder noot found");

        if (reminder.Planted != null && reminder.Planted.Place != null && reminder.Planted.Place.UserId != currentUser)
            throw new UnauthorizedAccessException("Cannot access this reminder");
    }
}
