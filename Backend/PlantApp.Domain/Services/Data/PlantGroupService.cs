using Microsoft.Extensions.Logging;
using PlantApp.Domain.Dtos.PlantGroup;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class PlantGroupService(
    IPlantGroupRepository repository,
    IPlantedRepository plantedRepo,
    IGrowthLogRepository logRepo,
    IReminderRepository reminderRepo,
    ICurrentUserContext userContext,
    ILogger<PlantGroupService> logger
) : IPlantGroupService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;
    public async Task<List<PlantGroupDto>> GetAllAsync()
    {
        int userId = CurrentUserId;
        var groups = await repository.GetAllByKeyAsync(p => p.UserId == userId, true);
        var sorted = groups.OrderBy(p => p.CreatedAt)
            .ToList();

        logger.LogInformation("Retrieved {Count} groups for user {UserId}", sorted.Count, userId);

        return sorted.Select(p => p.MapPlantGroupToPlantGroupDto()).ToList();
    }

    public async Task<PlantGroupGetDto> GetByIdAsync(int id)
    {
        var group = await repository.GetPlantGroupById(id);

        if (group == null) 
            throw new NotFoundException("Plant group", id, logger);

        var userId = CurrentUserId;
        if (group.UserId != userId && !IsAdmin) 
            throw new UnauthorizedException("access", "plant group", logger);

        group.GrowthLogs = await logRepo.GetAllGrowthLogsByPlantGroupId(id);
        var reminders = await reminderRepo.GetAllRemindersByPlantGroupId(id);

        var dto = group.MapPlantGroupToPlantGroupGetDto();
        dto.Reminders = reminders.Select(r => r.MapReminderToReminderGetDto()).ToList();

        return dto;
    }

    public async Task AddAsync(UpsertPlantGroupDto dto)
    {
        var group = dto.MapUpsertPlantGroupDtoToPlantGroup();
        group.UserId = CurrentUserId;

        await repository.AddAsync(group);

        logger.LogInformation("Plant Group {GroupId} added by user {UserId}", group.Id, CurrentUserId);
    }

    public async Task AddPlantsToGroupAsync(int id, List<int> plants)
    {
        var group = await repository.GetPlantGroupById(id);
        if (group == null)
            throw new NotFoundException("Plant group", id, logger);
        if (group.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("access", "plant group", logger);

        var newPlants = await plantedRepo.GetPlantedListByIdsAsync(plants);
        group.PlantedList.Clear();
        foreach (var plant in newPlants)
        {
            if (plant.Place == null || (plant.Place.UserId != CurrentUserId && !IsAdmin))
            {
                logger.LogWarning("Skipped plant {PlantId} due to ownership mismatch", plant.Id);
                continue;
            }

            /*if (!group.PlantedList.Any(p => p.Id == plant.Id))
                group.PlantedList.Add(plant);*/

            group.PlantedList.Add(plant);
        }

        await repository.UpdateAsync(group);

        logger.LogInformation("Plants added to Plant Group {GroupId} by user {UserId}", group.Id, CurrentUserId);
    }

    public async Task AddPlantToGroupAsync(int id, int plantId)
    {
        var group = await repository.GetPlantGroupById(id);
        if (group == null)
            throw new NotFoundException("Plant group", id, logger);
        if (group.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("access", "plant group", logger);

        var plant = await plantedRepo.GetByIdAsync(plantId);
        if (plant == null)
            throw new NotFoundException("Planted", plantId, logger);

        if (plant.Place.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("add plant", $"Plant Group {id}", logger);

        if (!group.PlantedList.Any(p => p.Id == plant.Id))
        {
            group.PlantedList.Add(plant);
            await repository.UpdateAsync(group);
        }

        logger.LogInformation("Plant {PlantId} added to Plant Group {GroupId} by user {UserId}", plantId, group.Id, CurrentUserId);
    }

    public async Task RemovePlantFromGroupAsync(int plantId)
    {
        var plant = await plantedRepo.GetByIdAsync(plantId);
        if (plant == null)
            throw new NotFoundException("Planted", plantId, logger);

        if (plant.Place.UserId != CurrentUserId && !IsAdmin)
            throw new UnauthorizedException("remove plant", $"Plant Group {plant.PlantGroupId}", logger);

        plant.PlantGroupId = null;
        await plantedRepo.UpdateAsync(plant);

        logger.LogInformation("Plant {PlantId} removed from Plant Group by user {UserId}", plantId, CurrentUserId);
    }

    public async Task UpdateAsync(int id, UpsertPlantGroupDto dto)
    {
        var existingGroup = await repository.GetByIdAsync(id);

        if (existingGroup == null) 
            throw new NotFoundException("Plant Group", id, logger);
        if (existingGroup.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "plant group", logger);

        dto.MapUpsertPlantGroupDtoToPlantGroup(existingGroup);
        await repository.UpdateAsync(existingGroup);

        logger.LogInformation("Plant Group {PlantGroupId} updated by user {UserId}", id, CurrentUserId);
    }

    public async Task DeleteAsync(int id)
    {
        var group = await repository.GetByIdAsync(id);

        if (group == null) 
            throw new NotFoundException("Plant Group", id, logger);
        if (group.UserId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("delete", "plant group", logger);

        if (group.PlantedList != null && group.PlantedList.Any())
        {
            throw new InvalidOperationAppException(
                userMessage: "This group cannot be deleted while it contains plants.",
                internalMessage: $"Group {id} has planted items and delete was attempted.",
                logger: logger
            );
        }

        await repository.DeleteAsync(group);
        logger.LogInformation("Plant Group {GroupId} deleted by user {UserId}", id, CurrentUserId);
    }
}
