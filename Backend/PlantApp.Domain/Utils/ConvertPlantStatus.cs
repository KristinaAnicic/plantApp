namespace PlantApp.Domain.Utils;

public static class ConvertPlantStatus
{
    public static (int Start, int End) SeasonIdToMonthRange(this int seasonId)
    {
        return seasonId switch
        {
            1 => (3, 5),   // Spring
            2 => (6, 8),   // Summer
            3 => (9, 11),  // Autumn
            4 => (12, 2),  // Winter
            _ => (1, 12)
        };
    }

    public static float MapStatusToScore(this int plantStatusId)
    {
        return plantStatusId switch
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
    }


}
