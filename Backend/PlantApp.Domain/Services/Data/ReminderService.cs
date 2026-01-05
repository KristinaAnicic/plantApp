using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using PlantApp.Data.Models;
using PlantApp.Data.Models.Categories;
using PlantApp.Domain.Dtos.Reminder;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class ReminderService(
    IReminderRepository repository,
    IRepository<ReminderHistory> reminderHistoryRepo,
    IRepository<ReminderType> reminderTypeRepo,
    IRepository<Frequency> frequencyRepo,
    IRepository<Planted> plantedRepo,
    ICurrentUserContext userContext,
    ILogger<ReminderService> logger
) : IReminderService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    public async Task<List<ReminderDto>> GetAllAsync()
    {
        var reminders = await repository.GetAllRemindersAsync(CurrentUserId);

        logger.LogInformation("Retrieved {Count} reminders for user {UserId}", reminders.Count, CurrentUserId);
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

        DateTime newDueDate = reminder.FrequencyTypeId switch
        {
            1 => dateDoneVar.AddDays(reminder.FrequencyNum),
            2 => dateDoneVar.AddDays(reminder.FrequencyNum * 7),
            3 => dateDoneVar.AddMonths(reminder.FrequencyNum),
            4 => dateDoneVar.AddYears(reminder.FrequencyNum),
            _ => throw new InvalidOperationException("Unknown frequency type for reminder.")
        };

        reminder.NextDueDate = newDueDate;
        reminder.DelayDays = 0;

        await repository.UpdateAsync(reminder);
        logger.LogInformation("Reminder {ReminderId} marked as done by user {UserId}", id, CurrentUserId);
    }

    public async Task DelayReminderAsync(int id, int delay)
    {
        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);

        reminder!.DelayDays = delay;
        reminder.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(reminder);

        logger.LogInformation("Reminder {ReminderId} delayed by {Delay} days by user {UserId}", id, delay, CurrentUserId);
    }
    public async Task AddAsync(UpsertReminderDto dto)
    {
        await ValidateReferences(dto);
        var reminder = dto.MapUpsertReminderDtoToReminder();

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId)
            ?? throw new KeyNotFoundException("Planted plant does not exist.");

        if (planted.Place!.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to add reminder for planted {PlantedId} without permission", CurrentUserId, dto.PlantedId);
            throw new UnauthorizedAccessException("You are not authorized to add a reminder for this planted plant.");
        }

        await repository.AddAsync(reminder);

        logger.LogInformation("Reminder {ReminderId} added for planted {PlantedId} by user {UserId}", reminder.Id, dto.PlantedId, CurrentUserId);
    }

    //on frontend disable changing nextDueDate or reminderHistory calculate delay by previous reminder
    public async Task UpdateAsync(int id, UpsertReminderDto dto)
    {
        if (dto.Id != id)
            throw new ArgumentException("DTO id does not match with provided id");

        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);
        await ValidateReferences(dto);

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId)
            ?? throw new KeyNotFoundException("Planted plant does not exist.");

        if (planted.Place!.UserId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to update reminder {ReminderId} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to update this reminder.");
        }

        dto.MapUpsertReminderDtoToReminder(reminder);

        await repository.UpdateAsync(reminder!);
        logger.LogInformation("Reminder {ReminderId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);

        await repository.DeleteAsync(reminder!, false);

        logger.LogInformation("Reminder {ReminderId} deleted by user {UserId}", id, CurrentUserId);
    }

    private async Task ValidateReferences(UpsertReminderDto dto)
    {
        if (!await frequencyRepo.IdExistsAsync(dto.FrequencyTypeId))
            throw new KeyNotFoundException("Frequency type does not exist.");

        if (!await reminderTypeRepo.IdExistsAsync(dto.ReminderTypeId))
            throw new KeyNotFoundException("Reminder type does not exist.");

        if (!await plantedRepo.IdExistsAsync(dto.PlantedId))
            throw new KeyNotFoundException("Planted plant does not exist.");
    }

    private void CheckReminderAndAuthorization(Reminder? reminder)
    {
        if (reminder == null)
            throw new KeyNotFoundException("Reminder not found.");

        if (reminder.Planted?.Place?.UserId != CurrentUserId)
            throw new UnauthorizedAccessException("You are not authorized to access this reminder.");
    }
}
