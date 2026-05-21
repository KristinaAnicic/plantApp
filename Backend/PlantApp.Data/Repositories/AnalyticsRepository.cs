using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.Analytics;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using Microsoft.Extensions.Logging;

namespace PlantApp.Data.Repositories;

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
                        p.Planted.Any(pl => pl.Place != null &&
                                           pl.Place.UserId == userId));

        var firstPlantedDate = await baseQueryPlanted
            .OrderBy(p => p.DatePlanted)
            .ThenBy(p => p.CreatedAt)
            .Select(p => (DateOnly?)p.DatePlanted)
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
        var stressed = new List<string> { "Sick", "Wilting", "Stressed" };

        var logs = await context.GrowthLogs
            .Where(h =>
                h.CreatedAt >= date &&
                h.PlantStatus != null &&
                h.DeletedAt == null &&
                h.PlantStatusId != 3 &&
                h.Planted.Any(pl =>
                    pl.Place != null &&
                    pl.Place.UserId == userId)
            )
            .OrderBy(h => h.ObservationDate)
            .SelectMany(h => h.Planted
                .Where(pl => pl.Place.UserId == userId)
                .Select(pgl => new
                {
                    PlantedId = pgl.Id,
                    h.ObservationDate,
                    StatusName = h.PlantStatus.Name
                }))
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
            .Where(log => log.DeletedAt == null &&
                      log.CreatedAt >= startDate &&
                      log.Planted.Any(pl =>
                          pl.Place != null &&
                          pl.Place.UserId == userId))
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

        if (query == null)
        {
            return (0, null);
        }

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

    public async Task<List<PercentageSegment>> GetSuccessForGroups(int userId)
    {
        var logs = await context.GrowthLogs
            .Where(l => l.DeletedAt == null &&
                ((l.PlantGroupId.HasValue && l.PlantGroup.DeletedAt == null && l.PlantGroup.UserId == userId) ||
                l.Planted.Any(pl => pl.DeletedAt == null && pl.PlantGroupId.HasValue && pl.Place.UserId == userId)))
            .Include(l => l.PlantGroup)
            .Include(l => l.Planted)
            .ToListAsync();

        var result = logs
            .SelectMany(l =>
                l.Planted.Any()
                ? l.Planted.Select(pl => new
                    {
                        PlantGroupName = pl.PlantGroup?.Name ?? l.PlantGroup?.Name ?? "No Group",
                        Score = l.PlantStatusId.MapStatusToScore()
                    })
                : new[] { new { 
                    PlantGroupName = l.PlantGroup?.Name ?? "No Group", 
                    Score = l.PlantStatusId.MapStatusToScore() 
                } }
            )
            .GroupBy(x => x.PlantGroupName)
            .Select(g => new PercentageSegment
            {
                Label = g.Key,
                Percentage = (int)Math.Round(g.Average(x => x.Score)),
            })
            .OrderBy(g => g.Percentage)
            .ToList();

        return result;
    }

    public async Task<List<PercentageSegment>> GetSuccessForFamily(int userId)
    {
        var logs = await context.GrowthLogs
            .Where(l => l.DeletedAt == null &&
                l.Planted.Any(pl => pl.DeletedAt == null && 
                pl.Plant != null && 
                pl.Plant.FamilyId.HasValue && 
                pl.Place.UserId == userId))
            .Include(l => l.Planted)
                .ThenInclude(p => p.Plant)
                    .ThenInclude(pl => pl.Family)
            .ToListAsync();

        var result = logs
            .SelectMany(l => l.Planted.Select(pl => new
                {
                    Family = pl.Plant?.Family?.Name ?? "Unknown",
                    Score = l.PlantStatusId.MapStatusToScore()
                })
            )
            .GroupBy(x => x.Family)
            .Select(g => new PercentageSegment
            {
                Label = g.Key,
                Percentage = (int)Math.Round(g.Average(x => x.Score)),
            })
            .OrderBy(g => g.Percentage)
            .ToList();

        return result;
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
