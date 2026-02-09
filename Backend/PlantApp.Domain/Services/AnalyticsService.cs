using PlantApp.Domain.Dtos.Analytics;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services;

public class AnalyticsService(
    IAnalyticsRepository repository,
    IMLRepository mLRepository,
    ICurrentUserContext userContext,
    IMLHealthPredictionService mlService,
): IAnalyticsService
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

        return new AnalyticsDto
        {
            Summary = summary,
            ReminderStats = reminderStats,
            HealthStats = healthStats,
            GrowthLogActivity = growthLogStats,
            ActionStats = actionStats,
            HallOfFame = hallOfFame,
            SeasonalPlanting = sesonalPlantings,
            HealthPrediction = healthPrediction
        };
    }

    private async Task<List<HealthPredictionDto>> GetHealthScorePredictions(int userId)
    {
        var rawData = await mLRepository.GetUserHealthPredictionInputData(userId);
        var results = new List<HealthPredictionDto>();

        var data = rawData.Select(d => new PlantHealthPredictionDto
        {
            PlaceName = d.PlaceName,
            PlantName = d.PlantName,
            MLInput = d.MapPlantAnalyticsRecordToPlantMLInput()
        }).ToList();

        foreach (var info in data)
        {
            var inputList = new List<HealthPredictionMLInput>();
            var startMonth = (int)info.MLInput.Month;

            for (int month = 0; month < 12; month++)
            {
                int currentMonth = ((startMonth - 1 + month) % 12) + 1;
                var inputCopy = info.MLInput.Clone();
                inputCopy.Month = (float)currentMonth;
                //inputCopy.DaysSincePlanted = info.MLInput.DaysSincePlanted + (month * 30);
                inputList.Add(inputCopy);
            }

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

        if (oldestPlant != null) {
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
}
