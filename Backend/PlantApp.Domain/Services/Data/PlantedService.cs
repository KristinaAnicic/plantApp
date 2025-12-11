using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class PlantedService(
    IPlantedRepository repository,
    IImageService imageService
) : IPlantedService
{
    public int currentUser = 0;
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
        var planted = await repository.GetByIdAsync(id);

        if (planted == null)
            throw new ArgumentException("Planted not found");
        
        if (planted.Reminders != null && planted.Reminders.Any())
            planted.Reminders = planted.Reminders.OrderBy(r => (r.NextDueDate.AddDays(r.DelayDays) - DateTime.UtcNow).TotalDays).ToList();

        return planted.MapPlantedToPlantedGetDto();
    }

    public async Task AddAsync(UpsertPlantedDto dto)
    {
        //temporary until jwt is implemented
        int currentUser = 0;

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
        //temporary until jwt is implemented
        int currentUser = 0;

        if (dto.Id != id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingPlanted = await repository.GetByIdAsync(id);

        if ( existingPlanted == null)
        {
            throw new ArgumentException("Planted plant with the provided Id does not exist.");
        }

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

    public async Task RemoveImageById(int plantedId, int imageId)
    {
        var planted = await repository.GetByIdAsync(plantedId);
        if (planted == null)
            throw new ArgumentException("Plant not found");

        if (planted.Place != null && planted.Place.UserId != currentUser)
            throw new InvalidOperationException("Access denied");

        await imageService.RemoveImageFromEntityAsync(planted, imageId);

        await repository.UpdateAsync(planted);
    }
}
