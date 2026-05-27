using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using PlantApp.Data;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Categories;

namespace PlantApp.Domain.Services;

public class SeedCsvDataService
{
    public static readonly string mainPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PlantApp.Data");
    public static readonly string csvPath = Path.Combine(mainPath, "csv");
    public static List<PlantSynonym> plantSynonymList = new List<PlantSynonym>();
    protected readonly AppDbContext context;
    public SeedCsvDataService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task SeedData()
    {
        var fragrances = await context.Fragrances.ToListAsync();
        if (!fragrances.Any())
            await SeedCsvData("fragrances", "fragrance_types.csv");

        var family = await context.PlantFamilies.ToListAsync();
        if(!family.Any())
            await SeedCsvData("plant_families", "plant_family.csv");

        var images = await context.Images.ToListAsync();
        if (!images.Any())
            await PopulateImages();

        var plants = await context.Plants.ToListAsync();
        if (!plants.Any())
            await SeedPlantData();
    }

    public async Task PopulateImages()
    {
        var plantImageFilePath = Path.Combine(csvPath, "plant_image.csv");
        var imageList = await GetPlantImages(plantImageFilePath);

        var existingNames = context.Images.Select(i => i.Url).ToHashSet();

        if (!imageList.Any()) return;

        foreach (var image in imageList)
        {
            if (string.IsNullOrEmpty(image.Url)) continue;

            if (!existingNames.Contains(image.Url))
            {
                context.Images.Add(
                    new Image {
                        Url = image.Url, 
                        Copyright = image.Copyright 
                    });
                existingNames.Add(image.Url);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task SeedCsvData(string tableName, string csvFile)
    {
        var filePath = Path.Combine(csvPath, csvFile);
        var lines = await File.ReadAllLinesAsync(filePath);

        foreach (var line in lines)
        {
            var parts = line.Split(',', 2);
            if (parts.Length != 2) continue;

            if (!int.TryParse(parts[0], out int id)) continue;
            string name = parts[1].Trim();
            if (string.IsNullOrEmpty(name)) continue;

            await context.Database.ExecuteSqlRawAsync(
                $"INSERT INTO {tableName} (id, name) OVERRIDING SYSTEM VALUE VALUES ({{0}}, {{1}}) ON CONFLICT (id) DO NOTHING",
                id, name);
        }
    }

    public async Task SeedPlantAttributeData()
    {
        var colorWithAttributeFile = Path.Combine(csvPath, "plant_colorWithAttribute.csv");
        var mappingFile = Path.Combine(csvPath, "plant_id_mapping.csv");

        var idMapping = File.ReadAllLines(mappingFile)
                            .Skip(1) //skip header
                            .Select(line => line.Split(','))
                            .ToDictionary(parts => int.Parse(parts[0]), parts => int.Parse(parts[1]));

        var plantIdsInDb = await context.Plants.Select(p => p.Id).ToListAsync();
        var plantIdsSet = new HashSet<int>(plantIdsInDb);

        using (var parser = new TextFieldParser(colorWithAttributeFile))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine(); // skip header

            var batchSize = 5000;
            var plantAttributesToAdd = new List<PlantSeasonAttribute>(batchSize);
            while (!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                var oldId = int.Parse(fields[0]);
                var seasonId = int.Parse(fields[1]);
                var colour = fields[2];
                var attributeTypeId = int.Parse(fields[3]);

                if (!idMapping.TryGetValue(oldId, out int newId))
                    continue;

                if (!plantIdsSet.Contains(newId))
                    continue;

                plantAttributesToAdd.Add(new PlantSeasonAttribute
                {
                    PlantId = newId,
                    SeasonId = seasonId,
                    Colour = colour,
                    PlantAttributeTypeId = attributeTypeId
                });

                if (plantAttributesToAdd.Count >= batchSize)
                {
                    context.PlantSeasonAttributes.AddRange(plantAttributesToAdd);
                    await context.SaveChangesAsync();
                    plantAttributesToAdd.Clear();
                }
            }

            if (plantAttributesToAdd.Any())
            {
                context.PlantSeasonAttributes.AddRange(plantAttributesToAdd);
                await context.SaveChangesAsync();
            }
        }
    }


    public async Task SeedPlantData()
    {
        var plantFilePath = Path.Combine(csvPath, "plant_data.csv");
        var plantAspectFilePath = Path.Combine(csvPath, "plant_aspect.csv");
        var plantExposureFilePath = Path.Combine(csvPath, "plant_exposure.csv");
        var plantHabitFilePath = Path.Combine(csvPath, "plant_habit.csv");
        var plantSeasonFilePath = Path.Combine(csvPath, "plant_seasonOfInterest.csv");
        var plantImageFilePath = Path.Combine(csvPath, "plant_image.csv");
        var plantMoistureFilePath = Path.Combine(csvPath, "plant_moisture.csv");
        var plantPhFilePath = Path.Combine(csvPath, "plant_ph.csv");
        var plantSoilTypeFilePath = Path.Combine(csvPath, "plant_soilType.csv");
        var plantSunlightFilePath = Path.Combine(csvPath, "plant_sunlight.csv");

        var aspectMap = BuildMap(plantAspectFilePath);
        var exposureMap = BuildMap(plantExposureFilePath);
        var habitMap = BuildMap(plantHabitFilePath);
        var seasonMap = BuildMap(plantSeasonFilePath);
        var moistureMap = BuildMap(plantMoistureFilePath);
        var phMap = BuildMap(plantPhFilePath);
        var soilTypeMap = BuildMap(plantSoilTypeFilePath);
        var sunlightMap = BuildMap(plantSunlightFilePath);
        var imageMap = await GetPlantImagesMap(plantImageFilePath);

        var imagesDict = await context.Images.ToDictionaryAsync(i => i.Url);
        var batch = new List<Batch>();
        int batchSize = 1000;

        using (TextFieldParser parser = new TextFieldParser(plantFilePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while(!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length == 0) continue;

                int originalPlantId = int.Parse(fields[0]);
                bool? isSynonym = ParseNullableBool(fields[1]);
                int? synonymParentId = ParseNullableInt(fields[2]);
                string? botanicalName = fields[3];
                bool? notedForFragnance = ParseNullableBool(fields[4]);
                int? fragranceId = ParseNullableInt(fields[5]);
                string? commonName = fields[6];
                int? hardinessLevelId = ParseNullableInt(fields[7]);
                bool? isGenus = ParseNullableBool(fields[8]);
                bool? isSpecie = ParseNullableBool(fields[9]);
                bool? isPlantForPollinators = ParseNullableBool(fields[10]) ;
                bool? isLowMaintenance = ParseNullableBool(fields[11]);
                bool? IsDroughtResistant = ParseNullableBool(fields[12]);
                int? spreadTypeId = ParseNullableInt(fields[13]);
                int? heightTypeId = ParseNullableInt(fields[14]);
                int timeToFullHeightId = int.Parse(fields[15]);
                int? foliageId = ParseNullableInt(fields[16]);
                string? toxicity = fields[17];
                string? cultivation = fields[18];
                string? pestResistance = fields[19];
                string? diseaseResistance = fields[20];
                string? pruning = fields[21];
                string? propagation = fields[22];
                int? familyId = ParseNullableInt(fields[23]);
                string? entityDescription = fields[24];
                string? genusDescription = fields[25];

                Plant plant = new Plant
                {
                    BotanicalName = botanicalName,
                    CommonName = commonName,
                    SynonymParentPlantId = null,
                    FragranceId = fragranceId,
                    HardinessLevelId = hardinessLevelId,
                    IsSpecie = isSpecie,
                    IsGenus = isGenus,
                    IsPlantForPollinators = isPlantForPollinators,
                    IsDroughtResistant = IsDroughtResistant,
                    IsLowMaintenance = isLowMaintenance,
                    SpreadTypeId = spreadTypeId,
                    HeightTypeId = heightTypeId,
                    TimeToFullHeightId = timeToFullHeightId,
                    Toxicity = toxicity,
                    Cultivation = cultivation,
                    PestResistance = pestResistance,
                    DiseaseResistance = diseaseResistance,
                    Pruning = pruning,
                    Propagation = propagation,
                    FamilyId = familyId,
                    EntityDescription = entityDescription,
                    GenusDescription = genusDescription
                };

                if (imageMap.TryGetValue(originalPlantId, out var plantImages))
                {
                    foreach (var image in plantImages)
                    {
                        if (!string.IsNullOrEmpty(image.Url) && imagesDict.TryGetValue(image.Url, out var existingImage))
                            plant.Images.Add(existingImage);
                    }
                }

                batch.Add(new Batch
                {
                    OldId = originalPlantId,
                    SynonymParentId = synonymParentId,
                    Plant = plant,
                    AspectIds = GetLookupIdsBySpecificPlantId(aspectMap, originalPlantId),
                    SoilTypeIds = GetLookupIdsBySpecificPlantId(soilTypeMap, originalPlantId),
                    SunlightIds = GetLookupIdsBySpecificPlantId(sunlightMap, originalPlantId),
                    MoistureIds = GetLookupIdsBySpecificPlantId(moistureMap, originalPlantId),
                    PhIds = GetLookupIdsBySpecificPlantId(phMap, originalPlantId),
                    ExposureIds = GetLookupIdsBySpecificPlantId(exposureMap, originalPlantId),
                    HabitIds = GetLookupIdsBySpecificPlantId(habitMap, originalPlantId),
                    SeasonIds = GetLookupIdsBySpecificPlantId(seasonMap, originalPlantId)
                });

                if (batch.Count >= batchSize)
                {            
                    await InsertBatch(batch);                    
                }
            }

            if (batch.Any())
            { 
                await InsertBatch(batch);
            }

            await AddSynonymParentIds(plantSynonymList);
        }
    }

    private async Task InsertBatch(List<Batch> batchList)
    {
        var allAspectIds = batchList.SelectMany(b => b.AspectIds).Distinct().ToList();
        var allSoilTypeIds = batchList.SelectMany(b => b.SoilTypeIds).Distinct().ToList();
        var allSunlightIds = batchList.SelectMany(b => b.SunlightIds).Distinct().ToList();
        var allMoistureIds = batchList.SelectMany(b => b.MoistureIds).Distinct().ToList();
        var allPhIds = batchList.SelectMany(b => b.PhIds).Distinct().ToList();
        var allExposureIds = batchList.SelectMany(b => b.ExposureIds).Distinct().ToList();
        var allHabitIds = batchList.SelectMany(b => b.HabitIds).Distinct().ToList();
        var allSeasonIds = batchList.SelectMany(b => b.SeasonIds).Distinct().ToList();

        var aspectsDict = await context.Aspects.Where(e => allAspectIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var soilTypesDict = await context.Soils.Where(e => allSoilTypeIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var sunlightsDict = await context.Sunlights.Where(e => allSunlightIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var moisturesDict = await context.Moistures.Where(e => allMoistureIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var phsDict = await context.Phs.Where(e => allPhIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var exposuresDict = await context.Exposures.Where(e => allExposureIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var habitsDict = await context.Habits.Where(e => allHabitIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);
        var seasonsDict = await context.Seasons.Where(e => allSeasonIds.Contains(e.Id)).ToDictionaryAsync(e => e.Id);

        foreach (var b in batchList)
        {
            AddRelationsFromCache(b.Plant.Aspects, b.AspectIds, aspectsDict);
            AddRelationsFromCache(b.Plant.SoilTypes, b.SoilTypeIds, soilTypesDict);
            AddRelationsFromCache(b.Plant.Sunlights, b.SunlightIds, sunlightsDict);
            AddRelationsFromCache(b.Plant.Moistures, b.MoistureIds, moisturesDict);
            AddRelationsFromCache(b.Plant.Phs, b.PhIds, phsDict);
            AddRelationsFromCache(b.Plant.Exposures, b.ExposureIds, exposuresDict);
            AddRelationsFromCache(b.Plant.Habits, b.HabitIds, habitsDict);
            AddRelationsFromCache(b.Plant.Seasons, b.SeasonIds, seasonsDict);
        }

        await context.Plants.AddRangeAsync(batchList.Select(x => x.Plant));
        await context.SaveChangesAsync();

        foreach (var b in batchList)
        {
            plantSynonymList.Add(new PlantSynonym
            {
                OriginalPlantId = b.OldId,
                NewPlantId = b.Plant.Id,
                OldSynonymParentId = b.SynonymParentId
            });
        }

        batchList.Clear();
    }

    private async Task AddSynonymParentIds(List<PlantSynonym> plantSynonymList)
    {
        if (plantSynonymList.Any())
        {
            var oldToNewId = plantSynonymList.ToDictionary(x => x.OriginalPlantId, x => x.NewPlantId);
            var plantDict = await context.Plants.ToDictionaryAsync(p => p.Id);
            List<Plant> updateList = new List<Plant>();

            foreach (var syn in plantSynonymList)
            {
                if (syn.OldSynonymParentId.HasValue && syn.OldSynonymParentId != 0)
                {

                    if (oldToNewId.TryGetValue(syn.OldSynonymParentId.Value, out var newSynonymParentId))
                    {
                        var plant = plantDict[syn.NewPlantId];
                        plant.SynonymParentPlantId = newSynonymParentId;
                        updateList.Add(plant);
                    }
                }
            }
            context.Plants.UpdateRange(updateList);
            await context.SaveChangesAsync();
        }
    }

    private Dictionary<int, List<int>> BuildMap(string filepath)
    {
        List<LookupRecord> idList = new List<LookupRecord>();

        foreach (var line in File.ReadLines(filepath))
        {
            var parts = line.Split(',', 2);
            if (parts.Length != 2) continue;

            int plantId = int.Parse(parts[0].Trim());
            int lookupId = int.Parse(parts[1].Trim());
            idList.Add(new LookupRecord { PlantId = plantId, LookupId = lookupId });
        }

        return idList.GroupBy(r => r.PlantId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => r.LookupId).ToList());
    }

    private List<int> GetLookupIdsBySpecificPlantId(Dictionary<int, List<int>> map, int plantId)
    {
        return map.TryGetValue(plantId, out var lookupIds) ? lookupIds : new List<int>();
    }



    public async Task<List<PlantImage>> GetPlantImages(string filepath)
    {
        List<PlantImage> imageList = new List<PlantImage>();

        var lines = await File.ReadAllLinesAsync(filepath);

        foreach (var line in lines)
        {
            var parts = line.Split(',', 3);
            if (parts.Length != 3) continue;

            int plantId = int.Parse(parts[0]);
            string image = parts[1];
            string copyright = parts[2];

            if (!string.IsNullOrWhiteSpace(image))
                imageList.Add(new PlantImage { 
                    PlantId = plantId,
                    Url = $"https://apps.rhs.org.uk/plantselectorimages/detail/{image}", 
                    Copyright = copyright 
                });
        }
        return imageList;
    }

    public async Task<Dictionary<int, List<PlantImage>>> GetPlantImagesMap(string filepath)
    {
        List<PlantImage> imageList = await GetPlantImages(filepath);
        return imageList
                .GroupBy(img => img.PlantId)
                .ToDictionary(
                    g => g.Key, 
                    g => g.ToList()
                );
    }

    private void AddRelationsFromCache<TEntity>(ICollection<TEntity> targetCollection,List<int> ids, Dictionary<int, TEntity> cache)
    {
        foreach (var id in ids)
        {
            if (cache.TryGetValue(id, out var entity))
            {
                targetCollection.Add(entity);
            }
        }
    }

    private int? ParseNullableInt(string value) { 
        if (int.TryParse(value, out var val) && val > 0)
            return val;
        return null;
    }

    private bool? ParseNullableBool(string value)
    {
        return bool.TryParse(value, out var val) ? val : null;
    }
    
}
public class LookupRecord
{
    public int PlantId { get; set; }
    public int LookupId { get; set; }
}

public class PlantImage
{
    public int PlantId { get; set; }
    public string? Url { get; set; }
    public string? Copyright { get; set; }
}

public class PlantSynonym
{
    public int OriginalPlantId { get; set; }
    public int NewPlantId { get; set; }
    public int? OldSynonymParentId { get; set; }
}

public class Batch
{
    public int OldId { get; set; }
    public int? SynonymParentId { get; set; }
    public required Plant Plant { get; set; }

    public List<int> AspectIds { get; set; } = new();
    public List<int> SoilTypeIds { get; set; } = new();
    public List<int> SunlightIds { get; set; } = new();
    public List<int> MoistureIds { get; set; } = new();
    public List<int> PhIds { get; set; } = new();
    public List<int> ExposureIds { get; set; } = new();
    public List<int> HabitIds { get; set; } = new();
    public List<int> SeasonIds { get; set; } = new();
}



