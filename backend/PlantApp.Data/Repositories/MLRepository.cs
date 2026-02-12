using Appwrite.Models;
using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;

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
            .Where(l => l.DeletedAt == null)
            .ToListAsync();

        var result = new List<HealthPredictionRecord>();

        foreach (var log in data)
        {
            var plants = new List<Planted>();

            if (log.PlantGroupId != null)
            {
                plants.AddRange(await context.Planteds
                    .Where(p => p.PlantGroupId == log.PlantGroupId &&
                                p.Plant != null &&
                                p.Plant.Family != null &&
                                p.Place != null &&
                                p.DeletedAt == null)
                    .ToListAsync());
            }
            else if (log.Planted != null &&
                log.Planted.Plant != null &&
                log.Planted.Plant.Family != null &&
                log.Planted.Place != null)
            {
                plants.Add(log.Planted);
            }

            foreach (var planted in plants)
            {
                if (planted?.Place == null || planted.Plant == null || planted.Plant.Family == null)
                    continue;

                var avgDelay = await context.ReminderHistory
                    .Where(r => r.PlantedId == planted.Id && DateOnly.FromDateTime(r.DueDate) <= log.ObservationDate)
                    .Select(r => (float?)r.delay)
                    .AverageAsync() ?? 0f;

                result.Add(new HealthPredictionRecord
                {
                    SunlightIntensity = (float)planted.Place!.SunlightIntensity,
                    HumidityIntensity = (float)planted.Place.HumidityIntensity,
                    IsOutside = planted.IsOutside,
                    Family = planted.Plant!.Family!.Name,
                    Hardiness = (float?)planted.Plant!.HardinessLevelId ?? 1f,
                    PlantStatusId = log.PlantStatusId,
                    SunlightList = planted.Plant.Sunlights.ToList(),
                    MoistureList = planted.Plant.Moistures.ToList(),
                    SeasonList = planted.Plant.Seasons.ToList(),
                    LowMaintenance = planted.Plant.IsLowMaintenance ?? false,
                    DroughtResistant = planted.Plant.IsDroughtResistant ?? false,
                    DaysSincePlanted = (float)(log.ObservationDate.DayNumber - planted.DatePlanted.DayNumber),
                    Month = log.ObservationDate.Month,
                    ReminderDelay = avgDelay
                });
            }
        }

        return result;
    }

    public async Task<List<HealthPredictionRecord>> GetUserHealthPredictionInputData(int userId, int? plantedId)
    {
        var query = context.Planteds
            .Where(p =>
                p.Place != null &&
                p.Plant != null &&
                p.Plant.Family != null &&
                p.DeletedAt == null &&
                p.PlantStatusId != 3);

        if (plantedId.HasValue)
        {
            query = query.Where(p => p.Id == plantedId.Value);
        }
        else
        {
            query = query.Where(p => p.Place!.UserId == userId);
        }

        return await query
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
                PlantedId = q.Planted.Id,

                SunlightIntensity = (float)q.Planted.Place!.SunlightIntensity,
                HumidityIntensity = (float)q.Planted.Place.HumidityIntensity,
                IsOutside = q.Planted.IsOutside,
                Family = q.Planted.Plant!.Family!.Name,
                Hardiness = (float?)q.Planted.Plant.HardinessLevelId ?? 1f,
                SunlightList = q.Planted.Plant.Sunlights.ToList(),
                MoistureList = q.Planted.Plant.Moistures.ToList(),
                SeasonList = q.Planted.Plant.Seasons.ToList(),
                LowMaintenance = q.Planted.Plant.IsLowMaintenance ?? false,
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
        var plantedList = await context.Planteds
            .Where(p => p.DeletedAt == null && p.Place != null && p.Plant != null)
            .Select(p => new RecommendationMLInput
            {
                PlantId = p.PlantId,
                UserId = p.Place.UserId,
                DaysAlive = (float)(p.DateOfDeath ?? today).DayNumber - p.DatePlanted.DayNumber,
            }).ToListAsync();

        return plantedList;
    }*/

    public async Task<List<RecommendationMLInput>> GetRecommendationMLInput()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var plantedList = await context.Planteds
            .Where(p => p.DeletedAt == null && 
                p.Place != null && p.Plant != null && 
                (p.DateOfDeath != null ||
                   (today.DayNumber - p.DatePlanted.DayNumber) >= 60
                ))
            .GroupBy(p => new { p.Place.UserId, PlantFamilyId = p.Plant.FamilyId })
            .Select(g => new RecommendationMLInput
            {
                PlantFamilyId = g.Key.PlantFamilyId ?? 0,
                UserId = g.Key.UserId,
                DaysAlive = g.Average(p => (float)((p.DateOfDeath ?? today).DayNumber - p.DatePlanted.DayNumber))
            })
            .ToListAsync();

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
}
