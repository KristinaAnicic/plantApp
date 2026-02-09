using Microsoft.ML;
using PlantApp.Domain.Interfaces.Repository;
using Microsoft.ML.Trainers;
using PlantApp.Domain.Dtos.ML;
using PlantApp.Domain.Interfaces;

namespace PlantApp.ML;

public class MLRecommendationService(IMLRepository MLRepository) : IMLRecommendationService
{
    private readonly string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "PlantApp",
        "MLModels",
        "PlantRecommendation.zip"
    );
    private ITransformer? _trainedModel;

    public async Task TrainModelAsync()
    {
        var data = await MLRepository.GetRecommendationMLInput();
        if (data.Count == 0) return;

        foreach (var row in data)
        {
            row.DaysAlive = (float)Math.Log(row.DaysAlive / (1 + 0.1f * row.AvgReminderDelay));
        }

        var context = new MLContext();
        IDataView dataView = context.Data.LoadFromEnumerable(data);

        var pipeline = context.Transforms.Conversion.MapValueToKey("UserIdEncoded", nameof(RecommendationMLInput.UserId))
            .Append(context.Transforms.Conversion.MapValueToKey("PlantFamilyIdEncoded", nameof(RecommendationMLInput.PlantFamilyId)))
            .Append(context.Recommendation().Trainers.MatrixFactorization(
                new MatrixFactorizationTrainer.Options
                {
                    MatrixColumnIndexColumnName = "UserIdEncoded",
                    MatrixRowIndexColumnName = "PlantFamilyIdEncoded",
                    LabelColumnName = "Label",
                    NumberOfIterations = 20,
                    ApproximationRank = 32
                }));

        var model = pipeline.Fit(dataView);

        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        context.Model.Save(model, dataView.Schema, path);
    }

    public DateTime GetModelCreationDate()
    {
        return File.GetLastWriteTimeUtc(path);
    }

    public async Task<List<string>> RecommendPlantsByuserIdAsync(int userId)
    {
        if (!File.Exists(path))
        {
            await TrainModelAsync();
            if (!File.Exists(path)) return new();
        }
        var context = new MLContext();

        if (_trainedModel == null)
        {
            _trainedModel = context.Model.Load(path, out _);
        }

        var input = await MLRepository.GetUserRecommendationInputData(userId);

        IDataView inputDataView = context.Data.LoadFromEnumerable(input);
        var predictions = _trainedModel.Transform(inputDataView);

        var predictedList = context.Data.CreateEnumerable<RecommendationPrediction>(predictions, reuseRowObject: false)
            .Select((p, index) => new
            {
                FamilyName = input[index].FamilyName,
                Score = p.Score
            })
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        return predictedList.Select(p => p.FamilyName!).ToList();
    }

}
