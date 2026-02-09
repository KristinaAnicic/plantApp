using Microsoft.ML.Data;

namespace PlantApp.Domain.Dtos.ML;

public class RecommendationMLInput
{
    public string? FamilyName { get; set; }
    public int PlantFamilyId { get; set; }
    public int UserId { get; set; }
    [ColumnName("Label")]
    public float DaysAlive { get; set; }
}
