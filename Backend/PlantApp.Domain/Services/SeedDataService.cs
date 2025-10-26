using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using PlantApp.Data;
using PlantApp.Data.Models;

namespace PlantApp.Domain.Services;

public class SeedDataService
{
    public static readonly string mainPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PlantApp.Data");
    public static readonly string csvPath = Path.Combine(mainPath, "csv");
    protected readonly AppDbContext context;
    public SeedDataService(AppDbContext context)
    {
        this.context = context;
    }

    public async Task SeedData()
    {
        await SeedCsvData("fragrances", "fragrance_types.csv");
        await SeedCsvData("plant_families", "plant_family.csv");
        await PopulateImages();
        await SeedPlantData();
    }

    public async Task PopulateImages()
    {
        var plantImageFilePath = Path.Combine(csvPath, "plant_image.csv");
        var imageList = await GetPlantImages(plantImageFilePath);

        var existingNames = context.Images.Select(i => i.Name).ToHashSet();

        if (!imageList.Any()) return;

        foreach (var image in imageList)
        {
            if (string.IsNullOrEmpty(image.Name)) continue;

            if (!existingNames.Contains(image.Name))
            {
                context.Images.Add(new Image { Name = image.Name, Copyright = image.Copyright });
                existingNames.Add(image.Name);
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task<List<M2MIds>> GetM2MIds(string filepath)
    {
        List<M2MIds> idList = new List<M2MIds>();

        var lines = await File.ReadAllLinesAsync(filepath);

        foreach (var line in lines)
        {
            var parts = line.Split(',', 2);
            if (parts.Length != 2) continue;

            int plantId = int.Parse(parts[0]);
            int m2mId = int.Parse(parts[1]);
            idList.Add(new M2MIds{ PlantId = plantId, M2mId = m2mId });
        }
        return idList;
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
               imageList.Add(new PlantImage { PlantId = plantId, Name = image, Copyright = copyright });
        }
        return imageList;
    }

    public async Task SeedPlantData()
    {
        var plantFilePath = Path.Combine(csvPath, "plant_data.csv");
        var plantAspectFilePath = Path.Combine(csvPath, "plant_aspect.csv");
        var plantExposureFilePath = Path.Combine(csvPath, "plant_exposure.csv");
        var plantHabitFilePath = Path.Combine(csvPath, "plant_habit.csv");
        var plantImageFilePath = Path.Combine(csvPath, "plant_image.csv");
        var plantMoistureFilePath = Path.Combine(csvPath, "plant_moisture.csv");
        var plantPhFilePath = Path.Combine(csvPath, "plant_ph.csv");
        var plantSoilTypeFilePath = Path.Combine(csvPath, "plant_soilType.csv");
        var plantSunlightFilePath = Path.Combine(csvPath, "plant_sunlight.csv");

        var aspectList = await GetM2MIds(plantAspectFilePath);
        var exposureList = await GetM2MIds(plantExposureFilePath);
        var habitList = await GetM2MIds(plantHabitFilePath);
        var moistureList = await GetM2MIds(plantMoistureFilePath);
        var phList = await GetM2MIds(plantPhFilePath);
        var soilTypeList = await GetM2MIds(plantSoilTypeFilePath);
        var sunlightList = await GetM2MIds(plantSunlightFilePath);
        var imageList = await GetPlantImages(plantImageFilePath);

        var aspectsDict = await context.Aspects.ToDictionaryAsync(a => a.Id);
        var soilTypesDict = await context.Soils.ToDictionaryAsync(s => s.Id);
        var sunlightsDict = await context.Sunlights.ToDictionaryAsync(s => s.Id);
        var moisturesDict = await context.Moistures.ToDictionaryAsync(m => m.Id);
        var phsDict = await context.Phs.ToDictionaryAsync(p => p.Id);
        var exposuresDict = await context.Exposures.ToDictionaryAsync(e => e.Id);
        var habitsDict = await context.Habits.ToDictionaryAsync(h => h.Id);
        var imagesDict = await context.Images.ToDictionaryAsync(i => i.Name);

        var batch = new List<(int OldId, int? SynonymParentId, Plant Plant)>();
        int batchSize = 1000;

        //var idMapping = new Dictionary<int, int>();
        List<PlantSynonym> plantSynonymList = new List<PlantSynonym>();

        using (TextFieldParser parser = new TextFieldParser(plantFilePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while(!parser.EndOfData)
            {
                var fields = parser.ReadFields();
                if (fields == null || fields.Length == 0) continue;

                int originalPlantId = int.Parse(fields[0]);
                bool? isSynonym = bool.TryParse(fields[1], out var s) ? s : null;
                int? synonymParentId = int.TryParse(fields[2], out var sId) ? (sId == 0 ? null : sId) : null;
                string? botanicalName = fields[3];
                bool? notedForFragnance = bool.TryParse(fields[4], out var nf) ? nf : null;
                int? fragranceId = int.TryParse(fields[5], out var fId) ? (fId == 0 ? null : fId) : null;
                string? commonName = fields[6];
                int? hardinessLevelId = int.TryParse(fields[7], out var hId) ? (hId == 0 ? null : hId) : null;
                bool? isGenus = bool.TryParse(fields[8], out var g) ? g : null;
                bool? isSpecie = bool.TryParse(fields[9], out var isp) ? isp : null;
                bool? isPlantForPollinators = bool.TryParse(fields[10], out var pp) ? pp : null ;
                bool? isLowMaintenance = bool.TryParse(fields[11], out var lm) ? lm : null;
                bool? isDroughtResistance = bool.TryParse(fields[12], out var dr) ? dr : null;
                int? spreadTypeId = int.TryParse(fields[13], out var spId) ? (spId == 0 ? null : spId) : null;
                int? heightTypeId = int.TryParse(fields[14], out var h) ? (h == 0 ? null : h) : null;
                int timeToFullHeightId = int.Parse(fields[15]);
                int? foliageId = int.TryParse(fields[16], out var foId) ? (foId == 0 ? null : foId) : null;
                string? toxicity = fields[17];
                string? cultivation = fields[18];
                string? pestResistance = fields[19];
                string? diseaseResistance = fields[20];
                string? pruning = fields[21];
                string? propagation = fields[22];
                int? familyId = int.TryParse(fields[23], out var fam) ? (fam == 0 ? null : fam) : null;
                string? entityDescription = fields[24];
                string? genusDescription = fields[25];

                var plant = new Plant
                {
                    BotanicalName = botanicalName,
                    CommonName = commonName,
                    SynonymParentPlantId = null,
                    FragranceId = fragranceId,
                    HardinessLevelId = hardinessLevelId,
                    IsSpecie = isSpecie,
                    IsGenus = isGenus,
                    IsPlantForPollinators = isPlantForPollinators,
                    IsDroughtResistance = isDroughtResistance,
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

                List<int> aspectIds = aspectList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> soilTypeIds = soilTypeList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> sunlightIds = sunlightList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> moistureIds = moistureList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> phIds = phList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> exposureIds = exposureList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                List<int> habitIds = habitList.Where(a => a.PlantId == originalPlantId) .Select(a => a.M2mId) .ToList();
                //List<Image> plantImageList = imageList.Where(a => a.PlantId == originalPlantId && a.Name != null).Select(a => new Image { Name = a.Name!, Copyright = a.Copyright}).ToList();

                AddRelationsFromCache(plant.Aspects, aspectIds, aspectsDict);
                AddRelationsFromCache(plant.SoilTypes, soilTypeIds, soilTypesDict);
                AddRelationsFromCache(plant.Sunlights, sunlightIds, sunlightsDict);
                AddRelationsFromCache(plant.Moistures, moistureIds, moisturesDict);
                AddRelationsFromCache(plant.Phs, phIds, phsDict);
                AddRelationsFromCache(plant.Exposures, exposureIds, exposuresDict);
                AddRelationsFromCache(plant.Habits, habitIds, habitsDict);


                foreach (var image in imageList.Where(a => a.PlantId == originalPlantId && !string.IsNullOrEmpty(a.Name)))
                {
                    if (string.IsNullOrEmpty(image.Name)) continue;
                    if (imagesDict.TryGetValue(image.Name!, out var existingImage))
                    {
                        plant.Images.Add(existingImage);
                    }
                }

                //context.Plants.Add(plant);
                batch.Add((originalPlantId, synonymParentId, plant));

                if (batch.Count >= batchSize)
                {
                    context.Plants.AddRange(batch.Select(x => x.Plant));
                    await context.SaveChangesAsync();

                    foreach (var (oldId, synParentId, p) in batch)
                    {
                        plantSynonymList.Add(new PlantSynonym { OriginalPlantId = oldId, PlantId = p.Id, SynonymParentId = synParentId });
                    }


                    batch.Clear();
                }
            }

            if (batch.Any())
            {
                context.Plants.AddRange(batch.Select(x => x.Plant));
                await context.SaveChangesAsync();

                foreach (var (oldId, synParentId, p) in batch)
                {
                    plantSynonymList.Add(new PlantSynonym { OriginalPlantId = oldId, PlantId = p.Id, SynonymParentId = synParentId });
                }
            }

            using (var writer = new StreamWriter(Path.Combine(csvPath, "plant_id_mapping.csv")))
            {
                writer.WriteLine("OldId,NewId,SynId");
                foreach (var p in plantSynonymList)
                {
                    writer.WriteLine($"{p.OriginalPlantId},{p.PlantId},{p.SynonymParentId}");
                }
            }
        }
    }

    private void AddRelationsFromCache<TEntity>(ICollection<TEntity> targetCollection,List<int> ids,Dictionary<int, TEntity> cache)
    {
        foreach (var id in ids)
        {
            if (cache.TryGetValue(id, out var entity))
            {
                targetCollection.Add(entity);
            }
        }
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
                $"INSERT INTO {tableName} (id, name, created_at) OVERRIDING SYSTEM VALUE VALUES ({{0}}, {{1}}, {{2}}) ON CONFLICT (id) DO NOTHING",
                id, name, DateTime.UtcNow);
        }
    }
}
public class M2MIds
{
    public int PlantId { get; set; }
    public int M2mId { get; set; }
}

public class PlantImage
{
    public int PlantId { get; set; }
    public string? Name { get; set; }
    public string? Copyright { get; set; }
}

public class PlantSynonym
{
    public int OriginalPlantId { get; set; }
    public int PlantId { get; set; }
    public int? SynonymParentId { get; set; }
}



