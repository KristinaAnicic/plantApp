using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Categories;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
using PlantApp.Domain.Models.Interfaces;

namespace PlantApp.Domain.Services.Data;

public class PlantService(
    IPlantRepository repository,
    IRepository<Fragrance> fragranceRepository,
    IRepository<HardinessLevel> hardinessRepository,
    IRepository<SpreadType> spreadRepository,
    IRepository<HeightType> heightRepository,
    IRepository<TimeToFullHeight> timeRepository,
    IRepository<PlantFamily> familyRepository,
    IRepository<SoilType> soilRepository,
    IRepository<Sunlight> sunlightRepository,
    IRepository<Aspect> aspectRepository,
    IRepository<Moisture> moistureRepository,
    IRepository<Ph> phRepository,
    IRepository<Exposure> exposureRepository,
    IRepository<Habit> habitRepository,
    IRepository<Season> seasonRepository,
    IImageService imageService,
    ICurrentUserContext userContext,
    ILogger<PlantService> logger
) : IPlantService
{

    private int CurrentUserId => userContext.GetCurrentUserId();
    public async Task<ListResponse<PlantDto>> GetAllAsync(int page)
    {
        (int total, var plants) = await repository.GetAllPlantsAsync(page);
        var dto = plants.Select(p => p.MapPlantToPlantDto()).ToList();

        logger.LogInformation("Retrieved {Count} plants for page {Page}", dto.Count, page);
        return new ListResponse<PlantDto> { Total = total, Items = dto };
    }

    public async Task<PlantGetDto?> GetByIdAsync(int id)
    {
        var plant = await repository.GetByIdAsync(id);

        if (plant == null) 
            throw new NotFoundException("Plant", id, logger);
        
        return plant.MapPlantToPlantGetDto();
    }

    public async Task<ListResponse<PlantDto>> GetFilteredAsync(FilterByDto filter, int page = 1)
    {
        (int total, var plants) = await repository.GetPlantsFiltered(filter, page);
        var dto = plants.Select(p => p.MapPlantToPlantDto()).ToList();

        logger.LogInformation("Retrieved {Count} filtered plants for page {Page}", dto.Count, page);
        return new ListResponse<PlantDto> { Total = total, Items = dto };
    }

    public async Task AddAsync(UpsertPlantDto plantDto) {
        if (plantDto.SynonymParentPlantId != null && !(await repository.IdExistsAsync(plantDto.SynonymParentPlantId.Value)))
        {
            logger.LogWarning("Synonym parent plant ID {ParentId} not found, ignoring", plantDto.SynonymParentPlantId);
            plantDto.SynonymParentPlantId = null;
        }

        plantDto.FragranceId = await ValidateForeignKeyAsync(plantDto.FragranceId, fragranceRepository);
        plantDto.HardinessLevelId = await ValidateForeignKeyAsync(plantDto.HardinessLevelId, hardinessRepository);
        plantDto.SpreadTypeId = await ValidateForeignKeyAsync(plantDto.SpreadTypeId, spreadRepository);
        plantDto.HeightTypeId = await ValidateForeignKeyAsync(plantDto.HeightTypeId, heightRepository);
        plantDto.FamilyId = await ValidateForeignKeyAsync(plantDto.FamilyId, familyRepository);

        if (!(await timeRepository.IdExistsAsync(plantDto.TimeToFullHeightId)))
        {
            throw new InvalidOperationAppException(
                userMessage: "Invalid plant data submitted (Time to full height).",
                internalMessage: $"TimeToFullHeight with id {plantDto.TimeToFullHeightId} does not exist.",
                logger: logger
            );
        }

        var plant = plantDto.MapUpsertPlantDtoToPlant();

        plant.SoilTypes = await soilRepository.GetByIdsAsync(plantDto.SoilTypes);
        plant.Sunlights = await sunlightRepository.GetByIdsAsync(plantDto.Sunlights);
        plant.Aspects = await aspectRepository.GetByIdsAsync(plantDto.Aspects);
        plant.Moistures = await moistureRepository.GetByIdsAsync(plantDto.Moistures);
        plant.Phs = await phRepository.GetByIdsAsync(plantDto.Phs);
        plant.Exposures = await exposureRepository.GetByIdsAsync(plantDto.Exposures);
        plant.Habits = await habitRepository.GetByIdsAsync(plantDto.Habits);
        plant.Seasons = await seasonRepository.GetByIdsAsync(plantDto.Seasons);


        if (plantDto.Images != null && plantDto.Images.Any())
        {
            plant.Images.Clear();
            await imageService.AddImagesSafeAsync(plant, plantDto.Images);
        }

        await repository.AddAsync(plant);

        logger.LogInformation("Plant {PlantId} added by user {UserId}", plant.Id, CurrentUserId);
    }

    public async Task UpdateAsync(int Id, UpsertPlantDto plantDto)
    {
        if (plantDto == null)
        {
            throw new InvalidOperationAppException(
                userMessage: "Invalid plant data submitted.",
                internalMessage: "Null UpsertPlantDto provided to UpdateAsync.",
                logger: logger
            );
        }

        if (plantDto.Id != Id) 
            throw new DtoIdMismatchException("Plant", plantDto.Id ?? 0, Id, logger);      

        var existingPlant = await repository.GetByIdAsync(Id);
        if (existingPlant == null) 
            throw new NotFoundException("Plant", Id, logger);
        
        if (plantDto.SynonymParentPlantId != null && !(await repository.IdExistsAsync(plantDto.SynonymParentPlantId.Value)))
        {
            plantDto.SynonymParentPlantId = existingPlant.SynonymParentPlantId;
        }

        plantDto.FragranceId = await ValidateForeignKeyAsync(plantDto.FragranceId, fragranceRepository) ?? existingPlant.FragranceId;
        plantDto.HardinessLevelId = await ValidateForeignKeyAsync(plantDto.HardinessLevelId, hardinessRepository) ?? existingPlant.HardinessLevelId;
        plantDto.SpreadTypeId = await ValidateForeignKeyAsync(plantDto.SpreadTypeId, spreadRepository) ?? existingPlant.SpreadTypeId;
        plantDto.HeightTypeId = await ValidateForeignKeyAsync(plantDto.HeightTypeId, heightRepository) ?? existingPlant.HeightTypeId;
        plantDto.FamilyId = await ValidateForeignKeyAsync(plantDto.FamilyId, familyRepository) ?? existingPlant.FamilyId;

        if (!(await timeRepository.IdExistsAsync(plantDto.TimeToFullHeightId)))
        {
            plantDto.TimeToFullHeightId = existingPlant.TimeToFullHeightId;
        }

        plantDto.MapUpsertPlantDtoToPlant(existingPlant);

        existingPlant.SoilTypes = await soilRepository.GetByIdsAsync(plantDto.SoilTypes);
        existingPlant.Sunlights = await sunlightRepository.GetByIdsAsync(plantDto.Sunlights);
        existingPlant.Aspects = await aspectRepository.GetByIdsAsync(plantDto.Aspects);
        existingPlant.Moistures = await moistureRepository.GetByIdsAsync(plantDto.Moistures);
        existingPlant.Phs = await phRepository.GetByIdsAsync(plantDto.Phs);
        existingPlant.Exposures = await exposureRepository.GetByIdsAsync(plantDto.Exposures);
        existingPlant.Habits = await habitRepository.GetByIdsAsync(plantDto.Habits);
        existingPlant.Seasons = await seasonRepository.GetByIdsAsync(plantDto.Seasons);

        if (plantDto.Images != null && plantDto.Images.Any())
        {
            existingPlant.Images.Clear();
            await imageService.AddImagesSafeAsync(existingPlant, plantDto.Images);
        }

        await repository.UpdateAsync(existingPlant);

        await imageService.RemoveUnusedImagesAsync();

        logger.LogInformation("Plant {PlantId} updated by user {UserId}", Id, CurrentUserId);
    }

    public async Task DeleteAsync(int Id)
    {
        var plant = await repository.GetByIdAsync(Id);
        if (plant == null) 
            throw new NotFoundException("Plant", Id, logger);
        
        if (plant.PlantedList != null && plant.PlantedList.Any())
        {
            await repository.DeleteAsync(plant);
            logger.LogWarning("Plant {PlantId} has planted instances so it is soft deleted by user {UserId}", Id, CurrentUserId);
            return;
        }

        await repository.DeleteAsync(plant, false);

        logger.LogInformation("Plant {PlantId} deleted by user {UserId}", Id, CurrentUserId);
    }

    public async Task AddImages(int plantId, List<string> urls)
    {
        var plant = await repository.GetByIdAsync(plantId);
        if (plant == null) 
            throw new NotFoundException("Plant", plantId, logger);

        await imageService.AddImagesToEntityAsync(plant, urls);
        await repository.UpdateAsync(plant);

        logger.LogInformation("Images added to plant {PlantId} by user {UserId}", plantId, CurrentUserId);
    }

    public async Task RemoveImageById(int plantId, int imageId)
    {
        var plant = await repository.GetByIdAsync(plantId);
        if (plant == null) 
            throw new NotFoundException("Plant", plantId, logger);

        await imageService.RemoveImageFromEntityAsync(plant, imageId, repository);
        //await repository.UpdateAsync(plant);

        logger.LogInformation("Image removed from plant {PlantId} by user {UserId}", plantId, CurrentUserId);
    }

    public async Task<ManyPlantAttributesDto> GetMultiReferenceDataAsync()
    {
        return new ManyPlantAttributesDto
        {
            Aspects = (await aspectRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            SoilTypes = (await soilRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Sunlights = (await sunlightRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Moistures = (await moistureRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Phs = (await phRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Exposures = (await exposureRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Habits = (await habitRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Seasons = (await seasonRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
        };
    }

    public async Task<OnePlantAttributesDto> GetSinglePlantReferenceDataAsync()
    {
        return new OnePlantAttributesDto
        {
            Fragrances = (await fragranceRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            HardinessLevels = (await hardinessRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            SpreadTypes = (await spreadRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            HeightTypes = (await heightRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            TimeToFullHeights = (await timeRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList(),
            Families = (await familyRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList()
        };
    }

    private async Task<int?> ValidateForeignKeyAsync<T>(int? id, IRepository<T> repo) where T : class
    {
        if (id == null) return null;
        return await repo.IdExistsAsync(id.Value) ? id : null;
    }
}
