using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Categories;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Dtos.Reminder;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

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
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<List<ReminderDto>> GetAllAsync()
    {
        var reminders = await repository.GetAllRemindersAsync(CurrentUserId);

        logger.LogInformation("Retrieved {Count} reminders for user {UserId}", reminders.Count, CurrentUserId);
        return reminders.Select(r => r.MapReminderToReminderDto()).ToList();
    }

    public async Task<List<ReminderDto>> GetPendingRemindersAsync()
    {
        var reminders = await repository.GetPendingRemindersAsync(CurrentUserId);

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

        DateTime baseDate = dateDone ?? DateTime.UtcNow;
        DateTime dateDoneVar = new DateTime(
            baseDate.Year,
            baseDate.Month,
            baseDate.Day,
            reminder!.NextDueDate.Hour,
            reminder.NextDueDate.Minute,
            0,
            DateTimeKind.Utc
        );

        var totalDelay = Math.Max(
            0,
            (int)(dateDoneVar.Date - reminder!.OriginalDueDate.Date).TotalDays
        );

        var reminderHistory = new ReminderHistory
        {
            PlantedId = reminder!.PlantedId,
            ReminderTypeId = reminder.ReminderTypeId,
            FrequencyTypeId = reminder.FrequencyTypeId,
            FrequencyNum = reminder.FrequencyNum,
            DueDate = reminder.OriginalDueDate,
            DateDone = dateDoneVar,
            delay = totalDelay
        };

        await reminderHistoryRepo.AddAsync(reminderHistory);

        DateTime newDueDate = reminder.FrequencyTypeId switch
        {
            1 => dateDoneVar.AddDays(reminder.FrequencyNum),
            2 => dateDoneVar.AddDays(reminder.FrequencyNum * 7),
            3 => dateDoneVar.AddMonths(reminder.FrequencyNum),
            4 => dateDoneVar.AddYears(reminder.FrequencyNum),
            _ => throw new InvalidOperationAppException(
                    userMessage: "Invalid reminder configuration.",
                    internalMessage: $"Reminder {id} has invalid FrequencyTypeId {reminder.FrequencyTypeId}",
                    logger: logger)
        };

        reminder.OriginalDueDate = DateTime.SpecifyKind(newDueDate, DateTimeKind.Utc);
        reminder.NextDueDate = reminder.OriginalDueDate;

        await repository.UpdateAsync(reminder);
        logger.LogInformation("Reminder {ReminderId} marked as done by user {UserId}", id, CurrentUserId);
    }

    public async Task DelayReminderAsync(int id, int delay)
    {
        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);

        DateTime baseDate = DateTime.UtcNow;
        DateTime today = new DateTime(
            baseDate.Year,
            baseDate.Month,
            baseDate.Day,
            reminder!.OriginalDueDate.Hour,
            reminder.OriginalDueDate.Minute,
            0,
            DateTimeKind.Utc
        );

        reminder!.NextDueDate = DateTime.SpecifyKind(today.AddDays(delay), DateTimeKind.Utc);
        reminder.UpdatedAt = DateTime.UtcNow;

        await repository.UpdateAsync(reminder);

        logger.LogInformation("Reminder {ReminderId} delayed by {Delay} days by user {UserId}", id, delay, CurrentUserId);
    }
    public async Task AddAsync(UpsertReminderDto dto)
    {
        await ValidateReferences(dto);
        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId)
            ?? throw new NotFoundException("Planted plant", dto.PlantedId, logger);

        if (planted.Place!.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("add", "reminder", logger);
        
        var reminder = dto.MapUpsertReminderDtoToReminder();

        await repository.AddAsync(reminder);

        logger.LogInformation("Reminder {ReminderId} added for planted {PlantedId} by user {UserId}", reminder.Id, dto.PlantedId, CurrentUserId);
    }

    //on frontend disable changing nextDueDate or reminderHistory calculate delay by previous reminder
    public async Task UpdateAsync(int id, UpsertReminderDto dto)
    {
        if (dto.Id != id) 
            throw new DtoIdMismatchException("Reminder", dto.Id, id, logger);

        var reminder = await repository.GetReminderAsync(id);

        CheckReminderAndAuthorization(reminder);
        await ValidateReferences(dto);

        var planted = await plantedRepo.GetByIdAsync(dto.PlantedId)
            ?? throw new NotFoundException("Planted plant", dto.PlantedId, logger);

        if (planted.Place!.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "reminder", logger); 

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

    public async Task<ReminderReferences> GetReferences()
    {
        var reminderTypes = await reminderTypeRepo.GetAllAsync();
        var frequencyTypes = await frequencyRepo.GetAllAsync();

        return new ReminderReferences
        {
            ReminderTypes = reminderTypes.Select(p => p.MapReferenceToDto()).ToList(),
            FrequencyTypes = frequencyTypes.Select(s => s.MapReferenceToDto()).ToList()
        };
    }

    private async Task ValidateReferences(UpsertReminderDto dto)
    {
        if (!await frequencyRepo.IdExistsAsync(dto.FrequencyTypeId)) 
            throw new NotFoundException("Frequency type", dto.FrequencyTypeId, logger);
        if (!await reminderTypeRepo.IdExistsAsync(dto.ReminderTypeId)) 
            throw new NotFoundException("Reminder type", dto.ReminderTypeId, logger);
        if (!await plantedRepo.IdExistsAsync(dto.PlantedId)) 
            throw new NotFoundException("Planted plant", dto.PlantedId, logger);
    }

    private void CheckReminderAndAuthorization(Reminder? reminder)
    {
        if (reminder == null) 
            throw new NotFoundException("Reminder", null, logger);
        if (reminder.Planted?.Place?.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("access", "reminder", logger);
    }
}
