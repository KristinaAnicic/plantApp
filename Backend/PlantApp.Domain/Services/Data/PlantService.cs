using PlantApp.Data.Models;
using PlantApp.Data.Models.Categories;
using PlantApp.Domain.Dtos;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

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
    IImageService imageService
) : IPlantService
{

    public int currentUser = 0;
    public async Task<ListResponse<PlantDto>> GetAllAsync(int page)
    {
        var plants = await repository.GetAllPlantsAsync(page);
        int total = await repository.CountAsync();

        var dto = plants.Select(p => p.MapPlantToPlantDto()).ToList();
        return new ListResponse<PlantDto> { Total = total, Items = dto };
    }

    public async Task<PlantGetDto?> GetByIdAsync(int id)
    {
        var plant = await repository.GetByIdAsync(id);
        return plant?.MapPlantToPlantGetDto();
    }

    public async Task<ListResponse<PlantDto>> GetFilteredAsync(FilterByDto filter, int page)
    {
        var plants = await repository.GetPlantsFiltered(filter, page);
        int total = await repository.CountAsync();

        var dto = plants.Select(p => p.MapPlantToPlantDto()).ToList();
        return new ListResponse<PlantDto> { Total = total, Items = dto };
    }

    public async Task AddAsync(UpsertPlantDto plantDto) {
        if (plantDto.SynonymParentPlantId != null && !(await repository.IdExistsAsync(plantDto.SynonymParentPlantId.Value)))
        {
            plantDto.SynonymParentPlantId = null;
        }

        plantDto.FragranceId = await ValidateForeignKeyAsync(plantDto.FragranceId, fragranceRepository);
        plantDto.HardinessLevelId = await ValidateForeignKeyAsync(plantDto.HardinessLevelId, hardinessRepository);
        plantDto.SpreadTypeId = await ValidateForeignKeyAsync(plantDto.SpreadTypeId, spreadRepository);
        plantDto.HeightTypeId = await ValidateForeignKeyAsync(plantDto.HeightTypeId, heightRepository);
        plantDto.FamilyId = await ValidateForeignKeyAsync(plantDto.FamilyId, familyRepository);

        if (!(await timeRepository.IdExistsAsync(plantDto.TimeToFullHeightId)))
        {
            throw new ArgumentNullException();
        }
        
        var plant = plantDto.MapUpsertPlantDtoToPlant();

        var soils = await soilRepository.GetByIdsAsync(plantDto.SoilTypes);
        plant.SoilTypes = soils;

        var sunlights = await sunlightRepository.GetByIdsAsync(plantDto.Sunlights);
        plant.Sunlights = sunlights;

        var aspects = await aspectRepository.GetByIdsAsync(plantDto.Aspects);
        plant.Aspects = aspects;

        var moistures = await moistureRepository.GetByIdsAsync(plantDto.Moistures);
        plant.Moistures = moistures;

        var phs = await phRepository.GetByIdsAsync(plantDto.Phs);
        plant.Phs = phs;

        var exposures = await exposureRepository.GetByIdsAsync(plantDto.Exposures);
        plant.Exposures = exposures;

        var habits = await habitRepository.GetByIdsAsync(plantDto.Habits);
        plant.Habits = habits;

        var seasons = await seasonRepository.GetByIdsAsync(plantDto.Seasons);
        plant.Seasons = seasons;


        if (plantDto.Images != null && plantDto.Images.Any())
        {
            plant.Images.Clear();

            await imageService.AddImagesSafeAsync(plant, plantDto.Images);
        }

        await repository.AddAsync(plant);
    }

    public async Task UpdateAsync(int Id, UpsertPlantDto plantDto)
    {
        if (plantDto == null)
            throw new ArgumentNullException(nameof(plantDto));

        if (plantDto.Id != Id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingPlant = await repository.GetByIdAsync(Id);

        if (existingPlant == null)
            throw new ArgumentException("Plant with the provided Id does not exist.");

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
    }

    public async Task DeleteAsync(int Id)
    {
        var plant = await repository.GetByIdAsync(Id);
        if (plant == null)
            throw new ArgumentException("Plant with the provided Id does not exist.");

        if (plant.PlantedList != null && plant.PlantedList.Any())
        {
            await repository.DeleteAsync(plant);
            return;
        }

        await repository.DeleteAsync(plant, false);
    }

    public async Task AddImages(int plantId, List<string> urls)
    {
        var plant = await repository.GetByIdAsync(plantId);
        if (plant == null)
            throw new ArgumentException("Plant not found");

        await imageService.AddImagesToEntityAsync(plant, urls);
        await repository.UpdateAsync(plant);
    }

    public async Task RemoveImageById(int plantId, int imageId)
    {
        var plant = await repository.GetByIdAsync(plantId);
        if (plant == null)
            throw new ArgumentException("Plant not found");

        await imageService.RemoveImageFromEntityAsync(plant, imageId);
        await repository.UpdateAsync(plant);
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
            Families = (await familyRepository.GetAllAsync()).Select(a => a.MapReferenceToDto()).ToList()
        };
    }

    private async Task<int?> ValidateForeignKeyAsync<T>(int? id, IRepository<T> repo) where T : class
    {
        if (id == null) return null;
        return await repo.IdExistsAsync(id.Value) ? id : null;
    }
}
