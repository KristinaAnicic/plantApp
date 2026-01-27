using Microsoft.EntityFrameworkCore;
using PlantApp.Data;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Analytics;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces.Data;

namespace PlantApp.Domain.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    public readonly AppDbContext context;
    public AnalyticsRepository(AppDbContext context)
    {
        this.context = context;
    }

    public async Task<PlantSummary> GetPlantSummary(int userId)
    {
        var baseQueryPlanted = context.Planteds
            .Where(p => p.DeletedAt == null && 
            p.Place != null && 
            p.Place.UserId == userId);

        var baseQueryLog = context.GrowthLogs
            .Where(p => p.DeletedAt == null &&
            p.Planted != null &&
            p.Planted.Place != null &&
            p.Planted.Place.UserId == userId);

        var firstPlantedDate = await baseQueryPlanted
            .OrderBy(p => p.DatePlanted)
            .ThenBy(p => p.CreatedAt)
            .Select(p => p.DatePlanted)
            .FirstOrDefaultAsync();

        var numOfPlants = await baseQueryPlanted.CountAsync();
        var numOfActivePlants = await baseQueryPlanted
            .Where(p => p.PlantStatusId != 3)
            .CountAsync();

        var numOfLogsOverAll = await baseQueryLog.CountAsync();
        var numOfLogsThisYear = await baseQueryLog
            .Where(l => l.CreatedAt.Year == DateTime.UtcNow.Year)
            .CountAsync();

        return new PlantSummary
        {
            NumOfCurrentPlants = numOfActivePlants,
            NumOfPlants = numOfPlants,
            NumOfLogsOverAll = numOfLogsOverAll,
            NumOfLogsThisYear = numOfLogsThisYear,
            FirstPlantedDate = firstPlantedDate
        };
    }

    public async Task<List<PercentageSegment>> GetReminderStats(int userId)
    {
        var date = DateTime.UtcNow.AddYears(-1);
        var query = context.ReminderHistory
            .Where(h => h.Planted != null && h.Planted.Place != null && h.Planted.Place.UserId == userId && h.CreatedAt >= date);

        var total = await query.CountAsync();
        if (total == 0) return new List<PercentageSegment>();

        return await query.GroupBy(h => h.delay == 0 ? "On Time" : h.delay <= 3 ? "Delayed" : "Late")
            .Select(g => new PercentageSegment
            {
                Label = g.Key,
                Percentage = (int)Math.Round((g.Count() * 100.0) / total)
            })
            .OrderByDescending(q => q.Percentage)
            .ToListAsync();
    }

    public async Task<List<PercentageSegment>> GetHealthStats(int userId)
    {
        var date = DateTime.UtcNow.AddYears(-1);
        var healthy = new List<string> { "Healthy", "Growing", "Flowering", "Fruiting", "Seedling", "Transplanted" };
        var stressed = new List<string> { "Sick", "Wilting", "Stressed", "Dormant" };

        var logs = await context.GrowthLogs
            .Where(h =>
                h.Planted != null &&
                h.Planted.Place != null &&
                h.Planted.Place.UserId == userId &&
                h.CreatedAt >= date &&
                h.PlantStatus != null &&
                h.DeletedAt == null
            )
            .OrderBy(h => h.PlantedId)
            .ThenBy(h => h.ObservationDate)
            .Select(h => new {
                h.PlantedId,
                h.ObservationDate,
                StatusName = h.PlantStatus.Name
            })
            .ToListAsync();

        if (!logs.Any()) return new List<PercentageSegment>();

        var durationPerStatus = new Dictionary<string, double>
        {
            { "Healthy", 0 },
            { "Stressed", 0 },
            { "Dormant", 0 }
        };

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        for (int i = 0; i < logs.Count; i++)
        {
            var currentLog = logs[i];
            DateOnly periodEnd;

            if (i + 1 < logs.Count && logs[i + 1].PlantedId == currentLog.PlantedId)
            {
                periodEnd = logs[i + 1].ObservationDate;
            }
            else
            {
                periodEnd = today;
            }

            int days = periodEnd.DayNumber - currentLog.ObservationDate.DayNumber;
            days = Math.Max(0, days);

            string status = healthy.Contains(currentLog.StatusName) ? "Healthy" :
                 stressed.Contains(currentLog.StatusName) ? "Stressed" : "Dormant";

            durationPerStatus[status] += days;
        }

        double totalDaysSum = durationPerStatus.Values.Sum();

        return durationPerStatus
             .Select(res => new PercentageSegment
             {
                 Label = res.Key,
                 Percentage = (int)Math.Round((res.Value * 100.0) / totalDaysSum)
             })
             .OrderByDescending(r => r.Percentage)
             .ToList();
    }

    public async Task<List<MonthlyActivityDto>> GetGrowthLogStats(int userId, DateTime startDate)
    {
        return await context.GrowthLogs
            .Where(log => log.Planted != null && log.Planted.Place != null && log.Planted.Place.UserId == userId && log.CreatedAt >= startDate && log.DeletedAt == null)
            .GroupBy(log => new { log.CreatedAt.Year, log.CreatedAt.Month })
            .Select(g => new MonthlyActivityDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(g => g.Year)
            .ThenBy(g => g.Month)
            .ToListAsync();
    }

    public async Task<List<ActionFrequencyDto>> GetActionFrequency(int userId)
    {
        var date = DateTime.UtcNow.AddYears(-1);
        return await context.ReminderHistory
            .Where(h => h.Planted != null && h.Planted.Place != null && h.Planted.Place.UserId == userId && h.CreatedAt >= date)
            .GroupBy(h => h.ReminderType.Name)
            .Select(g => new ActionFrequencyDto
            {
                ActionType = g.Key,
                Count = g.Count()
            })
            .ToListAsync();
    }

    public async Task<Planted?> GetOldestPlant(int userId)
    {
        var query = context.Planteds
            .Where(p => p.Place != null && p.Place.UserId == userId && p.DeletedAt == null)
            .OrderBy(h => h.DatePlanted)
            .ThenBy(h => h.CreatedAt);

        var projectedQuery = ProjectPlanted(query);
        return await projectedQuery.FirstOrDefaultAsync();
    }

    public async Task<(int, Planted?)> GetMostResilientPlant(int userId)
    {
        var query = await context.ReminderHistory
            .Where(h =>
                h.Planted != null &&
                h.Planted.DeletedAt == null &&
                h.Planted.Place != null &&
                h.Planted.Place.UserId == userId &&
                h.Planted.PlantStatusId != 3 &&
                h.delay > 3)
            .GroupBy(h => h.PlantedId)
            .Select(g => new {
                PlantedId = g.Key,
                Count = g.Count(),
                TotalDelayDays = g.Sum(h => h.delay)
            })
            .OrderByDescending(g => g.Count)
            .ThenByDescending(g => g.TotalDelayDays)
            .FirstOrDefaultAsync();

        var plant = context.Planteds
            .Where(p => p.Id == query.PlantedId);
        var projectedQuery = ProjectPlanted(plant);
        var planted = await projectedQuery.FirstOrDefaultAsync();

        var numOfMissed = query?.Count ?? 0;
        var totalMissedDays = query?.TotalDelayDays ?? 0;

        return (numOfMissed, planted);
    }

    public async Task<List<MonthlyActivityDto>> GetSeasonalNumOfPlantings(int userId)
    {
        return await context.Planteds
            .Where(p => p.Place != null && p.Place.UserId == userId && p.DeletedAt == null)
            .GroupBy(p => new { p.DatePlanted.Year, p.DatePlanted.Month})
            .Select(g => new MonthlyActivityDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count(),
            })
            .OrderBy(h => h.Year)
            .ThenBy(g => g.Month)
            .ToListAsync();
    }

    public async Task<List<PlantAnalyticsRecord>> GetTrainingData()
    {
        var data = await context.GrowthLogs
            .Where(l =>
                l.Place != null &&
                l.Planted != null &&
                l.Planted.Plant != null &&
                l.Planted.Plant.Family != null &&
                l.DeletedAt == null)
            .Select(l => new PlantAnalyticsRecord
            {
                SunlightIntensity = (float)l.Place!.SunlightIntensity,
                HumidityIntensity = (float)l.Place.HumidityIntensity,
                IsOutside = l.Planted!.IsOutside,
                Family = l.Planted.Plant!.Family!.Name,
                Hardiness = l.Planted.Plant.HardinessLevel != null ? l.Planted.Plant.HardinessLevel.Level : "Unknown",
                PlantStatusId = l.PlantStatusId,
                SunlightList = l.Planted.Plant.Sunlights.ToList(),
                MoistureList = l.Planted.Plant.Moistures.ToList(),
                LowMaintenace = l.Planted.Plant.IsLowMaintenance ?? false,
                DroughtResistant = l.Planted.Plant.IsDroughtResistant ?? false,
                Month = l.ObservationDate.Month
            }).ToListAsync();

        return data;
    }

    public async Task<List<PlantPredictionDto>> GetUserMLInputData(int userId)
    {
        var data = await context.Planteds
            .Where(p =>
                p.Place != null &&
                p.Plant != null &&
                p.Plant.Family != null &&
                p.DeletedAt == null &&
                p.PlantStatusId != 3 &&
                p.Place.UserId == userId)
            .Select(p => new
            {
                DisplayName = p.Name ?? p.Plant!.CommonName ?? p.Plant.BotanicalName,
                PlaceName = p.Place!.Name,
                SunlightIntensity = (float)p.Place!.SunlightIntensity,
                HumidityIntensity = (float)p.Place.HumidityIntensity,
                IsOutside = p.IsOutside,
                FamilyName = p.Plant!.Family!.Name,
                Hardiness = p.Plant.HardinessLevel != null ? p.Plant.HardinessLevel.Level : "Unknown",
                SunlightList = p.Plant.Sunlights.Select(s => "S" + s.Id).ToList(),
                MoistureList = p.Plant.Moistures.Select(m => "M" + m.Id).ToList(),
                LowMaintenace = p.Plant.IsLowMaintenance,
                DroughtResistant = p.Plant.IsDroughtResistant,
            })
            .ToListAsync();

        var results = data.Select(d => new PlantPredictionDto
        {
            PlantName = d.DisplayName,
            PlaceName = d.PlaceName,
            MLInput = new PlantMLInput
            {
                SunlightIntensity = d.SunlightIntensity,
                HumidityIntensity = d.HumidityIntensity,
                IsOutside = d.IsOutside,
                PlantFamily = d.FamilyName,
                HardinessLevel = d.Hardiness,
                HealthScore = 0,
                SunlightRequirements = string.Join(", ", d.SunlightList),
                MoistureRequirements = string.Join(", ", d.MoistureList),
                IsLowMaintenance = d.LowMaintenace ?? false,
                IsDroughtResistant = d.DroughtResistant ?? false,
                Month = (float)DateTime.UtcNow.Month
            }
        }).ToList();

        return results;
    }

    private IQueryable<Planted> ProjectPlanted(IQueryable<Planted> query)
    {
        return query.Select(q => new Planted
        {
            Id = q.Id,
            PlaceId = q.PlaceId,
            PlantId = q.PlantId,
            Name = q.Name ?? (q.Plant != null ? q.Plant.CommonName ?? q.Plant.BotanicalName : null),
            DatePlanted = q.DatePlanted,
            Image = q.Image ??
                    (q.Plant != null && q.Plant.Images.Any() ? q.Plant.Images.Select(i => i.Url).FirstOrDefault() :
                    q.Images.Any() ? q.Images.Select(i => i.Url).FirstOrDefault() : null)
        });
    }

}
