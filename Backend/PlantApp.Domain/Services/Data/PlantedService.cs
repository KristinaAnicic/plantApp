using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using System.Security;

namespace PlantApp.Domain.Services.Data;

public class PlantedService(
    IPlantedRepository repository,
    IImageService imageService,
    IRepository<Place> placeRepo,
    IRepository<PlantStatus> plantStatus
) : IPlantedService
{
    public int currentUser = 3;
    public async Task<List<PlantedDto>> GetAllByUserIdAsync(int userId)
    {
        var planted = await repository.GetPlantedPlantsByUserId(userId);
        return planted.Select(p => p.MapPlantedToPlantedDto()).ToList();
    }

    public async Task<List<GroupedPlantedDto>> GetAllByUserIdGroupedByPlaceAsync(int userId)
    {
        var planted = await repository.GetPlantedPlantsByUserIdGrouped(userId);
        var groupedDto = planted
            .Select(g => new GroupedPlantedDto 
            { 
                Place = g.Key.MapPlaceToPlaceDto(), 
                Planted = g.Value.Select(p => p.MapPlantedToPlantedDto()).ToList()
            })
            .ToList();

        return groupedDto;
    }

    public async Task<PlantedGetDto> GetByIdAsync(int id)
    {
        var planted = await repository.GetPlantedById(id);

        if (planted == null)
            throw new ArgumentException("Planted not found");
        
        if (planted.Reminders != null && planted.Reminders.Any())
            planted.Reminders = planted.Reminders.OrderBy(r => (r.NextDueDate.AddDays(r.DelayDays) - DateTime.UtcNow).TotalDays).ToList();

        return planted.MapPlantedToPlantedGetDto();
    }

    public async Task AddAsync(UpsertPlantedDto dto)
    {
        if (dto.DatePlanted == null)
            dto.DatePlanted = DateTime.UtcNow;

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);
        if (place == null)
            throw new ArgumentException("Place not found");

        if (place.UserId != currentUser)
            throw new AccessViolationException($"Not authorized to add plants to this place");

        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId)){
            throw new ArgumentException("Plant status not found");
        }

        var planted = dto.MapUpsertPlantedDtoToPlanted();

        if (dto.Images != null && dto.Images.Any())
        {
            planted.Images.Clear();

            await imageService.AddImagesSafeAsync(planted, dto.Images);
        }

        await repository.AddAsync(planted);
    }

    public async Task UpdateAsync(int id, UpsertPlantedDto dto)
    {
        if (dto.Id != id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingPlanted = await repository.GetByIdAsync(id);

        if ( existingPlanted == null)
        {
            throw new ArgumentException("Planted plant with the provided Id does not exist.");
        }

        var place = await placeRepo.GetByIdAsync(dto.PlaceId);
        if (place == null)
            throw new ArgumentException("Place not found");

        if (place.UserId != currentUser)
            throw new AccessViolationException("Not authorized to add plants to this place");

        if (!await plantStatus.IdExistsAsync(dto.PlantStatusId))
        {
            throw new ArgumentException("Plant status not found");
        }

        if (dto.DatePlanted == null)
            dto.DatePlanted = existingPlanted.DatePlanted;

        dto.MapUpsertPlantedDtoToPlanted(existingPlanted);

        if (dto.Images != null && dto.Images.Any())
        {
            existingPlanted.Images.Clear();

            await imageService.AddImagesSafeAsync(existingPlanted, dto.Images);
        }

        await repository.UpdateAsync(existingPlanted);
    }

    public async Task DeleteAsync(int id)
    {
        var planted = await repository.GetByIdAsync(id);

        if (planted == null)
        {
            throw new ArgumentException("Planted plant with the provided Id does not exist.");
        }   

        await repository.DeletePlantedAsync(planted);
    }

    public async Task AddImages(int plantedId, List<string> urls)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null)
            throw new ArgumentException("Plant not found");

        await imageService.AddImagesToEntityAsync(planted, urls);
        await repository.UpdateAsync(planted);
    }

    public async Task<string?> RemoveImageById(int plantedId, int imageId)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null)
            throw new ArgumentException("Plant not found");

        if (planted.Place != null && planted.Place.UserId != currentUser)
            throw new InvalidOperationException("Access denied");

        var deletedUrl = await imageService.RemoveImageFromEntityAsync(planted, imageId, repository);      
        //await repository.UpdateAsync(planted);

        return deletedUrl;
    }
}
