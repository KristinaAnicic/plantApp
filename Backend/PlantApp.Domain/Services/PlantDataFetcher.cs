using PlantApp.Domain.Dtos;
using PlantApp.Domain.Utils;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PlantApp.Domain.Services;

public static class PlantDataFetcher
{
    static readonly string mainPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PlantApp.Data");
    static readonly string jsonPath = Path.Combine(mainPath, "json_data");
    static readonly string csvPath = Path.Combine(mainPath, "csv");
    private static readonly Random rand = new Random();

    private static ConcurrentDictionary<int, string> FragnanceTypes
    = new ConcurrentDictionary<int, string>(CsvHelper.LoadCsvToDictionary(Path.Combine(csvPath, "fragrance_types.csv")));

    private static ConcurrentDictionary<int, string> PlantFamily
        = new ConcurrentDictionary<int, string>(CsvHelper.LoadCsvToDictionary(Path.Combine(csvPath, "plant_family.csv")));
   
    public static async Task FetchAllDataAsync()
    {
        var files = Directory.EnumerateFiles(jsonPath).OrderBy(f => f);
        var allIds = new List<int>();

        // fetch all ids from json files
        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            using var doc = JsonDocument.Parse(json);
            foreach (var hit in doc.RootElement.GetProperty("hits").EnumerateArray())
            {
                if (hit.TryGetProperty("id", out var idElem))
                    allIds.Add(idElem.GetInt32());
            }
        }

        File.AppendAllLines(Path.Combine(csvPath,"selected_files.txt"), files);


        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

        int processedCount = 0;

        using var writer = new StreamWriter(Path.Combine(csvPath,"plant_data.csv"), append: true) { AutoFlush = true };
        using var aspectWriter = new StreamWriter(Path.Combine(csvPath, "plant_aspect.csv"), append: true) { AutoFlush = true };
        using var moistureWriter = new StreamWriter(Path.Combine(csvPath, "plant_moisture.csv"), append: true) { AutoFlush = true };
        using var phWriter = new StreamWriter(Path.Combine(csvPath, "plant_ph.csv"), append: true) { AutoFlush = true };
        using var exposureWriter = new StreamWriter(Path.Combine(csvPath, "plant_exposure.csv"), append: true) { AutoFlush = true };
        using var plantTypeWriter = new StreamWriter(Path.Combine(csvPath, "plant_plantType.csv"), append: true) { AutoFlush = true };
        using var sunlightWriter = new StreamWriter(Path.Combine(csvPath, "plant_sunlight.csv"), append: true) { AutoFlush = true };
        using var soilTypeWriter = new StreamWriter(Path.Combine(csvPath, "plant_soilType.csv"), append: true) { AutoFlush = true };
        using var seasonWriter = new StreamWriter(Path.Combine(csvPath, "plant_seasonOfInterest.csv"), append: true) { AutoFlush = true };
        using var habitWriter = new StreamWriter(Path.Combine(csvPath, "plant_habit.csv"), append: true) { AutoFlush = true };
        using var imageWriter = new StreamWriter(Path.Combine(csvPath, "plant_image.csv"), append: true) { AutoFlush = true };
        using var colorAttributeWriter = new StreamWriter(Path.Combine(csvPath, "plant_colorWithAttribute.csv"), append: true) { AutoFlush = true };
        using var checkpointWriter = new StreamWriter(Path.Combine(csvPath, "checkpoint.txt"), append: true) { AutoFlush = true };
        using var fragnanceTypesWriter = new StreamWriter(Path.Combine(csvPath, "fragrance_types.csv"), append: true) { AutoFlush = true };
        using var plantFamilyWriter = new StreamWriter(Path.Combine(csvPath, "plant_family.csv"), append: true) { AutoFlush = true };

        foreach (int id in allIds)
        {
            try
            {
                var plant = await FetchPlantWithRetryAsync(http, id);
                if (plant != null)
                {
                    if (!string.IsNullOrEmpty(plant.fragrance) && !FragnanceTypes.Values.Contains(plant.fragrance))
                    {
                        int newKey = FragnanceTypes.Keys.DefaultIfEmpty(0).Max() + 1;
                        FragnanceTypes.TryAdd(newKey, plant.fragrance);

                        fragnanceTypesWriter.WriteLine($"{newKey},{plant.fragrance}");

                    }

                    if (!string.IsNullOrEmpty(plant.family) && !PlantFamily.Values.Contains(plant.family))
                    {
                        int newKey = PlantFamily.Keys.DefaultIfEmpty(0).Max() + 1;
                        PlantFamily.TryAdd(newKey, plant.family);

                        plantFamilyWriter.WriteLine($"{newKey},{plant.family}");
                    }

                    var record = new PlantResponseDto
                    {
                        Id = plant.id,
                        BotanicalName = plant.botanicalNameUnFormatted.RemoveHtmlLinks(),
                        IsSynonym = plant.isSynonym,
                        SynonymParentPlantId = plant.synonymParentPlantId,
                        NotedForFragrance = plant.notedForFragrance,
                        FragranceId = FragnanceTypes.FirstOrDefault(x => x.Value == plant.fragrance).Key,
                        CommonName = plant.commonName.RemoveHtmlLinks(),
                        HardinessLevel = plant.hardinessLevel,
                        IsGenus = plant.isGenus,
                        IsSpecie = plant.isSpecie,
                        IsPlantsForPollinators = plant.isPlantsForPollinators,
                        IsLowMaintenance = plant.isLowMaintenance,
                        IsDroughtResistance = plant.isDroughtResistance,
                        SpreadTypeId = plant.spreadType != null && plant.spreadType.Length > 0 ? plant.spreadType[0] : null,
                        HeightTypeId = plant.heightType != null && plant.heightType.Length > 0 ? plant.heightType[0] : null,
                        TimeToFullHeightId = plant.timeToFullHeight != null && plant.timeToFullHeight.Length > 0 ? plant.timeToFullHeight[0] : null,
                        FoliageId = plant.foliage != null && plant.foliage.Length > 0 ? plant.foliage[0] : null,
                        Toxicity = plant.toxicity != null ? string.Join(" | ", plant.toxicity).RemoveHtmlLinks() : null,
                        Cultivation = plant.cultivation.RemoveHtmlLinks(),
                        PestResistance = plant.pestResistance.RemoveHtmlLinks(),
                        DiseaseResistance = plant.diseaseResistance.RemoveHtmlLinks(),
                        Pruning = plant.pruning.RemoveHtmlLinks(),
                        Propagation = plant.propagation.RemoveHtmlLinks(),
                        FamilyId = PlantFamily.FirstOrDefault(x => x.Value == plant.family).Key,
                        EntityDescription = plant.entityDescription.RemoveHtmlLinks(),
                        GenusDescription = plant.genusDescription.RemoveHtmlLinks()
                    };
                    await writer.WriteLineAsync(record.ToCsvLine());

                    WriteRelation(aspectWriter, plant.id, plant.aspect);
                    WriteRelation(moistureWriter, plant.id, plant.moisture);
                    WriteRelation(phWriter, plant.id, plant.ph);
                    WriteRelation(exposureWriter, plant.id, plant.exposure);
                    WriteRelation(plantTypeWriter, plant.id, plant.plantType);
                    WriteRelation(sunlightWriter, plant.id, plant.sunlight);
                    WriteRelation(soilTypeWriter, plant.id, plant.soilType);
                    WriteRelation(seasonWriter, plant.id, plant.seasonOfInterest);
                    WriteRelation(habitWriter, plant.id, plant.habit);

                    if (plant.images != null)
                        foreach (var img in plant.images)
                            imageWriter.WriteLine($"{plant.id},{img.image},{img.copyRight}");

                    if (plant.colourWithAttributes != null)
                        foreach (var item in plant.colourWithAttributes)
                            colorAttributeWriter.WriteLine($"{plant.id},{item.season},{item.colour},{item.attributeType}");

                    checkpointWriter.WriteLine(id);
                    checkpointWriter.Flush();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ID {id} failed: {ex.Message}");
                File.AppendAllText(Path.Combine(csvPath, "failed_ids.txt"), id + Environment.NewLine);

            }

            processedCount++;
            DisplayProgress(processedCount, allIds.Count);

            int delayMs = 1500 + rand.Next(0, 1500);
            await Task.Delay(delayMs);
        } 
        Console.WriteLine("Scraping is finished");
    }

    private static async Task<DataApiResponse?> FetchPlantWithRetryAsync(HttpClient http, int id)
    {
        int maxRetries = 5;
        int attempt = 0;

        while (true)
        {
            try
            {
                var url = $"https://lwapp-uks-prod-psearch-01.azurewebsites.net/api/v1/plants/details/{id}";
                var response = await http.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DataApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else if ((int)response.StatusCode == 429 || (int)response.StatusCode >= 500)
                {
                    attempt++;
                    if (attempt > maxRetries) throw new Exception($"Max retries exceeded for {id}");
                    int wait = (int)Math.Pow(2, attempt) * 1000 + rand.Next(0, 500);
                    await Task.Delay(wait);
                }
                else
                {
                    throw new Exception($"HTTP {response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                attempt++;
                if (attempt > maxRetries) throw;
                int wait = (int)Math.Pow(2, attempt) * 1000 + rand.Next(0, 500);
                await Task.Delay(wait);
            }
        }
    }

    private static void WriteRelation(StreamWriter writer, int plantId, int[]? values)
    {
        if (values != null)
            foreach (var v in values)
                writer.WriteLine($"{plantId},{v}");
    }

    private static void DisplayProgress(int processed, int total)
    {
        int width = 50;
        double pct = (double)processed / total;
        int progressChars = (int)(pct * width);

        string bar = "[" + new string('#', progressChars) + new string('-', width - progressChars) + $"] {pct:P0} ({processed}/{total})";
        Console.CursorLeft = 0;
        Console.Write(bar);
    }

    public static string? RemoveHtmlLinks(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string result = Regex.Replace(input, @"<a[^>]*>", "", RegexOptions.IgnoreCase);

        result = result.Replace("</a>", "", StringComparison.OrdinalIgnoreCase);
        result = result.Replace("\"", "\"\"");

        return result;
    }

    public static void CheckIds(int numOfFiles)
    {
        var ids = CsvHelper.ReadIdsFromCsv(Path.Combine(csvPath,"plant_data.csv"));
        

        var files = Directory.EnumerateFiles(jsonPath).OrderBy(f => f).Take(numOfFiles);
        var allIds = new List<int>();

        foreach (var file in files)
        {
            var json = File.ReadAllText(file);
            using var doc = JsonDocument.Parse(json);
            foreach (var hit in doc.RootElement.GetProperty("hits").EnumerateArray())
            {
                if (hit.TryGetProperty("id", out var idElem))
                    allIds.Add(idElem.GetInt32());
            }
        }

        Console.WriteLine($"There are {ids.Count}/{allIds.Count} IDs.");

        var missing = allIds.Except(ids).ToList();
        if (missing.Any())
        {
            Console.WriteLine("Missing IDs: " + string.Join(", ", missing.Take(50)));
            //File.WriteAllLines(Path.Combine(csvPath,"missing_ids.txt"), missing.Select(x => x.ToString()));
        }
    }   
}
