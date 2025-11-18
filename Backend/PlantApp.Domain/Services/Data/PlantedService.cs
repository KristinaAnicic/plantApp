using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class PlantedService(
    IPlantedRepository repository,
    IRepository<Image> imageRepository
) : IPlantedService
{
    public async Task<List<PlantedDto>> GetPlantedPlants(int userId)
    {
        var planted = await repository.GetPlantedPlantsByUserId(userId);
        return planted.Select(p => p.MapPlantedToPlantedDto()).ToList();
    }

    public async Task<List<GroupedPlantedDto>> GetPlantedPlantsGroupedByPlace(int userId)
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

    public async Task AddPlanted(UpsertPlantedDto dto)
    {
        //temporary until jwt is implemented
        int currentUser = 0;

        var planted = dto.MapUpsertPlantedDtoToPlanted();

        if (dto.Images != null && dto.Images.Any())
        {
            var images = await imageRepository.GetByIdsAsync(dto.Images);
            images = images.Where(im => im.UserId == currentUser || im.UserId == null).ToList();
            planted.Images = images;
        }

        await repository.AddAsync(planted);
    }

    public async Task UpdatePlanted(int id, UpsertPlantedDto dto)
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
            var images = await imageRepository.GetByIdsAsync(dto.Images);
            images = images.Where(im => im.UserId == currentUser || im.UserId == null).ToList();
            existingPlanted.Images = images;
        }

        await repository.UpdateAsync(existingPlanted);
    }

    public async Task DeletePlanted(int id)
    {
        var planted = await repository.GetByIdAsync(id);

        if (planted == null)
        {
            throw new ArgumentException("Planted plant with the provided Id does not exist.");
        }   

        await repository.DeletePlantedAsync(planted);
    }
}
