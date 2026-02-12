using Appwrite;
using Appwrite.Models;
using Microsoft.Extensions.Logging;
using PlantApp.Domain.Dtos.Analytics;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using PlantApp.Domain.Models.Categories;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services;

public class AnalyticsService(
    IAnalyticsRepository repository,
    IMLRepository mLRepository,
    ICurrentUserContext userContext,
    IMLHealthPredictionService mlService,
    IMLRecommendationService mlRecService,
    IPlantedRepository plantedRepo,
    IPlantRepository plantRepo,
    IGrowthLogRepository logRepo,
    IPlantGroupRepository groupRepo,
    ILogger<AnalyticsService> logger
) : IAnalyticsService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;

    public async Task<AnalyticsDto> GetAnalytics()
    {
        var userId = CurrentUserId;

        var summary = await repository.GetPlantSummary(userId);
        var reminderStats = await repository.GetReminderStats(userId);
        var healthStats = await repository.GetHealthStats(userId);
        var growthLogStats = await GetGrowthLogStats(userId);
        var sesonalPlantings = await GetSeasonalPlantings(userId);
        var actionStats = await repository.GetActionFrequency(userId);
        var hallOfFame = await GetHallOfFame(userId);
        var healthPrediction = await GetHealthScorePredictions(userId);
        var recommendations = await GetPlantRecommendations(userId);

        return new AnalyticsDto
        {
            Summary = summary,
            ReminderStats = reminderStats,
            HealthStats = healthStats,
            GrowthLogActivity = growthLogStats,
            ActionStats = actionStats,
            HallOfFame = hallOfFame,
            SeasonalPlanting = sesonalPlantings,
            HealthPrediction = healthPrediction,
            PlantRecommendations = recommendations
        };
    }

    public async Task<PlantedAnalyticsDto> GetPlantedAnalytics(int plantedId)
    {
        var userId = CurrentUserId;
        var planted = await plantedRepo.GetPlantedById(plantedId);

        if (planted == null)
            throw new NotFoundException("Planted plant", plantedId, logger);

        if (planted.Place == null || (planted.Place.UserId != userId && !IsAdmin))
            throw new UnauthorizedException("delete", "planted", logger);

        var healthPrediction = await GetHealthScorePredictionsForPlanted(userId, plantedId);
        var growthStats = await GetPlantGrowthOverYearAsync(plantedId, DateTime.UtcNow.Year);

        return new PlantedAnalyticsDto
        {
            MonthlyHealthPrediction = healthPrediction,
            PlantGrowthHeight = growthStats
        };
    }

    public async Task<PlantGroupAnalytics> GetPlantedGroupAnalytics(int plantGroupId, int? year = null)
    {
        var userId = CurrentUserId;
        var group = await groupRepo.GetPlantGroupById(plantGroupId);

        if (group == null)
            throw new NotFoundException("Plant group", plantGroupId, logger);

        if (group.UserId != userId && !IsAdmin)
            throw new UnauthorizedException("access", "group", logger);

        var growthList = await GetGroupedPlantGrowthAnalytics(group, DateTime.UtcNow.Year);
        var monthlyAvgPerMonth = await GetGroupedLogAnalytics(plantGroupId);

        return new PlantGroupAnalytics { 
            GroupLogAnalytics = monthlyAvgPerMonth,
            GrowthAnalytics = growthList
        };
    }

    private float MapStatusToScore(GrowthLog log) => log.PlantStatusId switch
    {
        6 or 7 or 9 => 100f,
        1 or 5 => 85f,
        8 => 75f,
        11 => 70f,
        12 => 50f,
        10 => 40f,
        4 => 20f,
        2 => 10f,
        3 => 0f,
        _ => 50f
    };

    private async Task<List<GroupedGrowthAnalytics>> GetGroupedPlantGrowthAnalytics(PlantGroup group, int year)
    {
        var growthList = new List<GroupedGrowthAnalytics>();

        foreach (var planted in group.PlantedList)
        {
            var growthStats = await GetPlantGrowthOverYearAsync(planted.Id, year);
            growthList.Add(new GroupedGrowthAnalytics
            {
                Planted = planted.MapPlantedToPlantedDto(),
                PlantGrowthHeight = growthStats
            });
        }

        return growthList;
    }

    private async Task<List<PlantGroupLogAnalytics>> GetGroupedLogAnalytics(int plantGroupId)
    {
        var logs = await logRepo.GetAllGrowthLogsByPlantGroupId(plantGroupId);
        var logsOrdered = logs.OrderBy(l => l.ObservationDate).ToList();

        float lastKnownHealth = 0f;
        var monthlyAnalytics = new List<PlantGroupLogAnalytics>();

        if (logsOrdered.Any())
        {
            var startDate = new DateTime(logsOrdered.First().ObservationDate.Year, logsOrdered.First().ObservationDate.Month, 1);
            var endDate = new DateTime(logsOrdered.Last().ObservationDate.Year, logsOrdered.Last().ObservationDate.Month, 1);

            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var logsThisMonth = logsOrdered
                    .Where(l => l.ObservationDate.Year == currentDate.Year && l.ObservationDate.Month == currentDate.Month)
                    .ToList();

                float avgHealth = logsThisMonth.Any()
                    ? logsThisMonth.Select(MapStatusToScore).Average()
                    : lastKnownHealth;

                monthlyAnalytics.Add(new PlantGroupLogAnalytics
                {
                    Month = currentDate.Month,
                    AvgHealth = avgHealth
                });

                lastKnownHealth = avgHealth;
                currentDate = currentDate.AddMonths(1);
            }
        }

        var monthlyAvgPerMonth = monthlyAnalytics
            .GroupBy(m => m.Month)
            .Select(g => new PlantGroupLogAnalytics
            {
                Month = g.Key,
                AvgHealth = g.Average(x => x.AvgHealth)
            })
            .OrderBy(m => m.Month)
            .ToList();

        return monthlyAvgPerMonth;
    }

    private async Task<List<float>> GetHealthScorePredictionsForPlanted(int userId, int plantedId)
    {
        var rawData = await mLRepository.GetUserHealthPredictionInputData(userId, plantedId);
        var results = new List<HealthPredictionDto>();

        var record = rawData.FirstOrDefault();
        if (record == null)
            return new List<float>();

        var mlInput = record.MapPlantAnalyticsRecordToPlantMLInput();
        var inputList = BuildMonthlyInputs(mlInput);

        return await mlService.PredictHealthScoresBatch(inputList);
    }


    private async Task<List<HealthPredictionDto>> GetHealthScorePredictions(int userId)
    {
        var rawData = await mLRepository.GetUserHealthPredictionInputData(userId, null);
        var results = new List<HealthPredictionDto>();

        var data = rawData.Select(d => new PlantHealthPredictionDto
        {
            PlaceName = d.PlaceName,
            PlantName = d.PlantName,
            MLInput = d.MapPlantAnalyticsRecordToPlantMLInput()
        }).ToList();

        foreach (var info in data)
        {
            var inputList = BuildMonthlyInputs(info.MLInput);

            var monthlyPrediction = await mlService.PredictHealthScoresBatch(inputList);
            var currentScore = monthlyPrediction.FirstOrDefault();

            results.Add(new HealthPredictionDto
            {
                PlaceName = info.PlaceName,
                PlantName = info.PlantName,
                CurrentSuccessProbability = currentScore,
                MonthlyPrediction = monthlyPrediction
            });
        }
        return results;
    }

    private List<HealthPredictionMLInput> BuildMonthlyInputs(HealthPredictionMLInput baseInput)
    {
        var inputs = new List<HealthPredictionMLInput>();
        int startMonth = Math.Clamp((int)baseInput.Month, 1, 12);

        for (int i = 0; i < 12; i++)
        {
            int currentMonth = ((startMonth - 1 + i) % 12) + 1;
            var copy = baseInput.Clone();
            copy.Month = currentMonth;

            inputs.Add(copy);
        }
        return inputs;
    }


    private async Task<List<MonthlyActivityDto>> GetGrowthLogStats(int userId)
    {
        var now = DateTime.UtcNow;
        var startDate = new DateTime(now.Year - 1, now.Month, 1).AddMonths(1);
        var newDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);

        var allMonthStats = new List<MonthlyActivityDto>();

        var results = await repository.GetGrowthLogStats(userId, newDate);
        for (int i = 0; i < 12; i++)
        {
            var targetDate = startDate.AddMonths(i);
            var existingData = results.FirstOrDefault(r => r.Month == targetDate.Month && r.Year == targetDate.Year);

            allMonthStats.Add(new MonthlyActivityDto
            {
                Month = targetDate.Month,
                Year = targetDate.Year,
                Count = existingData?.Count ?? 0
            });
        }

        return allMonthStats;
    }

    private async Task<List<MonthlyActivityDto>> GetSeasonalPlantings(int userId)
    {
        var allMonthStats = new List<MonthlyActivityDto>();

        var results = await repository.GetSeasonalNumOfPlantings(userId);
        if (!results.Any()) return allMonthStats;

        var firstEntry = results.OrderBy(r => r.Year).ThenBy(r => r.Month).First();
        var startDate = new DateTime(firstEntry.Year, firstEntry.Month, 1);
        var endDate = DateTime.UtcNow;

        int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;

        for (int i = 0; i < totalMonths; i++)
        {
            var targetDate = startDate.AddMonths(i);
            var existingData = results.FirstOrDefault(r => r.Month == targetDate.Month && r.Year == targetDate.Year);

            allMonthStats.Add(new MonthlyActivityDto
            {
                Month = targetDate.Month,
                Year = targetDate.Year,
                Count = existingData?.Count ?? 0
            });
        }

        return allMonthStats;
    }

    private async Task<PlantHallOfFame> GetHallOfFame(int userId)
    {
        var oldestPlant = await repository.GetOldestPlant(userId);
        var daysAlive = 0;

        if (oldestPlant != null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            daysAlive = today.DayNumber - oldestPlant.DatePlanted.DayNumber;
        }

        var (totalMissed, mostResilientPlant) = await repository.GetMostResilientPlant(userId);

        return new PlantHallOfFame
        {
            OldestPlant = oldestPlant != null ? oldestPlant.MapPlantedToPlantedDto() : null,
            DaysAlive = daysAlive,
            MostResilientPlant = mostResilientPlant != null ? mostResilientPlant.MapPlantedToPlantedDto() : null,
            NumOfLateReminder = totalMissed,
        };
    }

    private async Task<List<string>> GetPlantRecommendations(int userId)
    {
        var planted = await plantedRepo.GetPlantedPlantsByUserId(userId);
        if (planted == null || planted.Count == 0)
        {
            return await plantRepo.GetTopPlantFamilies();
        }

        var modelDate = mlRecService.GetModelCreationDate();
        var minPlantAgeDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-90));

        int validPlantsCount = planted.Count(p =>
            p.CreatedAt < modelDate &&
            p.DatePlanted <= minPlantAgeDate);

        if (validPlantsCount >= 3)
        {
            logger.LogInformation("AI Plant Recommendation");
            return await mlRecService.RecommendPlantsByuserIdAsync(userId);
        }

        logger.LogInformation("Showing Top Plants");
        return await plantRepo.GetTopPlantFamilies();

    }

    private async Task<List<PlantGrowthHeight>> GetPlantGrowthOverYearAsync(int plantedId, int year)
    {
        var planted = await plantedRepo.GetPlantedForGrowthStatisticsAsync(plantedId);
        if (planted == null || planted.Plant == null)
            return new List<PlantGrowthHeight>();

        var seasons = planted.Plant.Seasons.ToList() ?? new List<Season>();
        var chartData = new List<PlantGrowthHeight>();

        decimal lastHeight = CalculateLastHeight(planted, seasons, year);

        int startMonth = planted.DatePlanted.Year == year ? planted.DatePlanted.Month : 1;

        for (int month = startMonth; month <= 12; month++)
        {
            var date = new DateTime(year, month, 15);
            int seasonId = date.Month switch
            {
                3 or 4 or 5 => 1,       // Spring
                6 or 7 or 8 => 2,       // Summer
                9 or 10 or 11 => 3,     // Autumn
                _ => 4,                 // Winter
            };

            bool isSeasonActive = seasons.Any(s => s.Id == seasonId);

            decimal height;
            if (isSeasonActive)
            {
                if (planted.Plant.TimeToFullHeight?.MaxTime <= 1)
                    height = CalculateSeasonalHeight(planted, month, seasons);

                else
                {
                    height = CalculateHeight(planted, DateOnly.FromDateTime(date));
                }

                lastHeight = height;
            }
            else
            {
                if (planted.Plant.TimeToFullHeight?.MaxTime > 1)
                    height = lastHeight;
                else
                    height = 0;
            }


            var attributesThisMonth = planted.Plant.PlantSeasonAttributes
                .Where(a => a.SeasonId == seasonId)
                .Select(a => a.PlantAttributeType?.Name ?? "")
                .Distinct()
                .ToList();

            chartData.Add(new PlantGrowthHeight
            {
                Month = date.Month,
                Height = Math.Round(height, 2),
                ActiveAttributes = attributesThisMonth
            });
        }

        return chartData;
    }

    private decimal CalculateSeasonalHeight(Planted planted, int currentMonth, List<Season> seasons)
    {
        if (planted?.Plant == null) return 0;

        var plant = planted.Plant;
        var minHeight = plant.HeightType?.MinHeight ?? 0;
        var maxHeight = plant.HeightType?.MaxHeight ?? minHeight * 1.5m;
        decimal avgHeight = (minHeight + maxHeight) / 2m;

        List<int> activeMonths = new List<int>();

        foreach (var season in seasons)
        {
            var (start, end) = SeasonIdToMonthRange(season.Id);
            if (end >= start)
            {
                for (int m = start; m <= end; m++)
                    activeMonths.Add(m);
            }
            else
            {
                for (int m = start; m <= 12; m++)
                    activeMonths.Add(m);
                for (int m = 1; m <= end; m++)
                    activeMonths.Add(m);
            }
        }

        activeMonths = activeMonths.Distinct().OrderBy(m => m).ToList();
        int firstActiveMonth = activeMonths.First();
        int lastActiveMonth = activeMonths.Last();

        int monthsSinceSeasonStart = currentMonth - firstActiveMonth + 1;
        if (monthsSinceSeasonStart <= 0) monthsSinceSeasonStart = 0;

        int seasonLength = lastActiveMonth - firstActiveMonth + 1;
        if (seasonLength <= 0) seasonLength = 12;

        /*decimal estimatedHeight = avgHeight * monthsSinceSeasonStart / seasonLength;
        if (estimatedHeight > maxHeight) estimatedHeight = avgHeight;*/

        decimal growthProgress = (decimal)monthsSinceSeasonStart / seasonLength;
        if (growthProgress > 1) growthProgress = 1;

        decimal growthFactor;
        if (growthProgress <= 0.5m)
            growthFactor = 2 * growthProgress;
        else
            growthFactor = 1m;

        decimal estimatedHeight = avgHeight * growthFactor;

        if (estimatedHeight > maxHeight) estimatedHeight = maxHeight;
        if (estimatedHeight < minHeight) estimatedHeight = minHeight;

        return estimatedHeight;
    }

    private decimal CalculateHeight(Planted planted, DateOnly date)
    {
        if (planted?.Plant == null) return 0;

        var plant = planted.Plant;
        var minHeight = plant.HeightType?.MinHeight ?? 0;
        var maxHeight = plant.HeightType?.MaxHeight ?? minHeight * 1.5m;

        decimal avgHeight = (minHeight + maxHeight) / 2m;

        decimal yearsSincePlanted = (decimal)((date.DayNumber - planted.DatePlanted.DayNumber) / 365m);
        if (yearsSincePlanted <= 0) return 0;

        decimal maxYears = (decimal)(plant.TimeToFullHeight?.MaxTime ?? yearsSincePlanted);
        if (yearsSincePlanted >= maxYears)
            return avgHeight;

        decimal estimatedHeight = yearsSincePlanted / maxYears * avgHeight;
        return estimatedHeight;
    }

    private decimal CalculateLastHeight(Planted planted, List<Season> seasons, int year)
    {
        if (planted.DatePlanted.Year >= year)
            return 0;

        if (seasons.Count == 0)
            return CalculateHeight(planted, new DateOnly(year, 1, 1));

        var lastSeason = seasons
            .Select(s => SeasonIdToMonthRange(s.Id))
            .OrderByDescending(r => r.End)
            .First();

        int lastMonth = lastSeason.End == 2 ? 12 : lastSeason.End;

        var lastActiveDate = new DateOnly(year - 1, lastMonth, 15);

        return CalculateHeight(planted, lastActiveDate);
    }

    (int Start, int End) SeasonIdToMonthRange(int seasonId) => seasonId switch
    {
        1 => (3, 5),   // Spring
        2 => (6, 8),   // Summer
        3 => (9, 11),  // Autumn
        4 => (12, 2),  // Winter
        _ => (1, 12)
    };

}
