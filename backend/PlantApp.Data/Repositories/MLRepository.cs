using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces.Repository;

namespace PlantApp.Data.Repositories;

public class MLRepository : IMLRepository
{
    public readonly AppDbContext context;
    public MLRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<List<HealthPredictionRecord>> GetHealthPredictionTrainingData()
    {
        var data = await context.GrowthLogs
            .Where(l =>
                l.Place != null &&
                l.Planted != null &&
                l.Planted.Plant != null &&
                l.Planted.Plant.Family != null &&
                l.DeletedAt == null)
            .Select(l => new
            {
                Log = l,
                AvgDeleay = context.ReminderHistory
                    .Where(r => r.PlantedId == l.PlantedId && DateOnly.FromDateTime(r.DueDate) <= l.ObservationDate)
                    .Select(r => (float?)r.delay)
                    .Average() ?? 0f
            })
            .Select(q => new HealthPredictionRecord
            {
                SunlightIntensity = (float)q.Log.Place!.SunlightIntensity,
                HumidityIntensity = (float)q.Log.Place.HumidityIntensity,
                IsOutside = q.Log.Planted!.IsOutside,
                Family = q.Log.Planted.Plant!.Family!.Name,
                Hardiness = q.Log.Planted.Plant.HardinessLevel != null ? q.Log.Planted.Plant.HardinessLevel.Level : "Unknown",
                PlantStatusId = q.Log.PlantStatusId,
                SunlightList = q.Log.Planted.Plant.Sunlights.ToList(),
                MoistureList = q.Log.Planted.Plant.Moistures.ToList(),
                SeasonList = q.Log.Planted.Plant.Seasons.ToList(),
                LowMaintenace = q.Log.Planted.Plant.IsLowMaintenance ?? false,
                DroughtResistant = q.Log.Planted.Plant.IsDroughtResistant ?? false,
                DaysSincePlanted = (float)(q.Log.ObservationDate.DayNumber - q.Log.Planted.DatePlanted.DayNumber),
                Month = q.Log.ObservationDate.Month,
                ReminderDelay = q.AvgDeleay
            }).ToListAsync();

        return data;
    }

    public async Task<List<HealthPredictionRecord>> GetUserHealthPredictionInputData(int userId)
    {
        return await context.Planteds
            .Where(p =>
                p.Place != null &&
                p.Plant != null &&
                p.Plant.Family != null &&
                p.DeletedAt == null &&
                p.PlantStatusId != 3 &&
                p.Place.UserId == userId)
            .Select(p => new
            {
                Planted = p,
                AvgDeleay = context.ReminderHistory
                    .Where(r => r.PlantedId == p.Id)
                    .Select(r => (float?)r.delay)
                    .Average() ?? 0f
            })
            .Select(q => new HealthPredictionRecord
            {
                PlantName = q.Planted.Name ?? q.Planted.Plant!.CommonName ?? q.Planted.Plant.BotanicalName,
                PlaceName = q.Planted.Place!.Name,

                SunlightIntensity = (float)q.Planted.Place!.SunlightIntensity,
                HumidityIntensity = (float)q.Planted.Place.HumidityIntensity,
                IsOutside = q.Planted.IsOutside,
                Family = q.Planted.Plant!.Family!.Name,
                Hardiness = q.Planted.Plant.HardinessLevel != null ? q.Planted.Plant.HardinessLevel.Level : "Unknown",
                SunlightList = q.Planted.Plant.Sunlights.ToList(),
                MoistureList = q.Planted.Plant.Moistures.ToList(),
                SeasonList = q.Planted.Plant.Seasons.ToList(),
                LowMaintenace = q.Planted.Plant.IsLowMaintenance ?? false,
                DroughtResistant = q.Planted.Plant.IsDroughtResistant ?? false,
                Month = (float)DateTime.UtcNow.Month,
                DaysSincePlanted = (float)(DateOnly.FromDateTime(DateTime.UtcNow).DayNumber - q.Planted.DatePlanted.DayNumber),
                ReminderDelay = q.AvgDeleay,
                HealthScore = 0
            })
            .ToListAsync();
    }

    /*public async Task<List<RecommendationMLInput>> GetRecommendationMLInput()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var plantCount = await GetPlantCountsAsync();

        var plantedList = await context.Planteds
            .Where(p => p.DeletedAt == null && p.Place != null && p.Plant != null)
            .Select(p => new RecommendationMLInput
            {
                PlantId = p.PlantId,
                UserId = p.Place.UserId,
                DaysAlive = (float)(p.DateOfDeath ?? today).DayNumber - p.DatePlanted.DayNumber,
                AvgReminderDelay = p.ReminderHistory.Select(r => (float?)r.delay).Average() ?? 0f,
                IsLowMaintenance = p.Plant.IsLowMaintenance ?? false,
            }).ToListAsync();

        foreach (var plant in plantedList)
        {
            plant.TimesPlanted = plantCount.GetValueOrDefault(plant.PlantFamilyId, 0);
        }

        return plantedList;
    }*/

    public async Task<List<RecommendationMLInput>> GetRecommendationMLInput()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var plantCount = await GetPlantCountsAsync();

        var plantedList = await context.Planteds
            .Where(p => p.DeletedAt == null && p.Place != null && p.Plant != null)
            .GroupBy(p => new { p.Place.UserId, PlantFamilyId = p.Plant.FamilyId })
            .Select(g => new RecommendationMLInput
            {
                PlantFamilyId = g.Key.PlantFamilyId ?? 0,
                UserId = g.Key.UserId,
                DaysAlive = g.Sum(p => (float)((p.DateOfDeath ?? today).DayNumber - p.DatePlanted.DayNumber)),
                AvgReminderDelay = g.SelectMany(p => p.ReminderHistory).Select(r => (float?)r.delay).DefaultIfEmpty(0f).Average() ?? 0f,
                TimesPlanted = g.Count(),
                IsLowMaintenance = g.First().Plant.IsLowMaintenance ?? false
            }).ToListAsync();

        foreach (var plant in plantedList)
        {
            plant.TimesPlanted = plantCount.GetValueOrDefault(plant.PlantFamilyId, 0);
        }

        return plantedList;
    }

    /*public async Task<List<RecommendationMLInput>> GetUserRecommendationInputData(int userId)
    {
        var plantList = await context.Plants
            .Where(p => p.DeletedAt == null && 
                !p.PlantedList.Any(pl => pl.Place != null && pl.Place.UserId == userId))
            .Select(p => new RecommendationMLInput
            {
                PlantName = p.Name,
                PlantId = p.Id,
                UserId = userId,
                DaysAlive = 0
            }).ToListAsync();

        return plantList;
    }*/

    public async Task<List<RecommendationMLInput>> GetUserRecommendationInputData(int userId)
    {
        var plantList = await context.PlantFamilies
            .Where(fam => 
                !fam.Plants.Any(p => p.PlantedList.Any(pl => pl.Place != null && pl.Place.UserId == userId)))
            .Select(fam => new RecommendationMLInput
            {
                FamilyName = fam.Name,
                PlantFamilyId = fam.Id,
                UserId = userId,
                DaysAlive = 0
            }).ToListAsync();

        return plantList;
    }

    private async Task<Dictionary<int, int>> GetPlantCountsAsync()
    {
        return await context.Planteds
            .Where(p => p.DeletedAt == null && p.Plant != null && p.Plant.FamilyId != null)
            .GroupBy(p => p.Plant.FamilyId!.Value)
            .Select(g => new { FamilyId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.FamilyId, x => x.Count);
    }

}
