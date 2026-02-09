using Microsoft.ML;
using Microsoft.ML.Data;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Dtos.ML;

namespace PlantApp.Domain.Services;

public class MLHealthPredictionService(IMLRepository MLRepository) : IMLHealthPredictionService
{
    private readonly string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        "PlantApp",
        "MLModels",
        "HealthPredictionModel.zip"
    );
    private ITransformer? _trainedModel;
    public async Task TrainModelAsync()
    {
        
        var rawData = await MLRepository.GetHealthPredictionTrainingData();
        if (rawData.Count == 0) return;

        var dataRecord = rawData.Select(d => new HealthPredictionRecord
        {
            SunlightIntensity = d.SunlightIntensity,
            HumidityIntensity = d.HumidityIntensity,
            IsOutside = d.IsOutside,
            Family = d.Family,
            Hardiness = d.Hardiness,
            PlantStatusId = d.PlantStatusId,
            SunlightList = d.SunlightList,
            MoistureList = d.MoistureList,
            SeasonList = d.SeasonList,
            LowMaintenace = d.LowMaintenace,
            DroughtResistant = d.DroughtResistant,
            DaysSincePlanted = d.DaysSincePlanted,
            ReminderDelay = d.ReminderDelay,
            Month = d.Month,

            HealthScore = CalculateAdjustedHealthScore(d)
        }).ToList();

        var data = dataRecord
            .Select(d => d.MapPlantAnalyticsRecordToPlantMLInput())
            .ToList();

        var context = new MLContext();
        IDataView dataView = context.Data.LoadFromEnumerable(data);

        //var split = context.Data.TrainTestSplit(dataView, testFraction: 0.2);

        var pipeline = context.Transforms
            .Categorical.OneHotEncoding("FamilyEncoded", nameof(HealthPredictionMLInput.PlantFamily))
            .Append(context.Transforms.Categorical.OneHotEncoding("HardinessEncoded", nameof(HealthPredictionMLInput.HardinessLevel)))

            .Append(context.Transforms.Conversion.ConvertType(nameof(HealthPredictionMLInput.IsOutside), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(HealthPredictionMLInput.IsLowMaintenance), outputKind: DataKind.Single))
            .Append(context.Transforms.Conversion.ConvertType(nameof(HealthPredictionMLInput.IsDroughtResistant), outputKind: DataKind.Single))
            .Append(context.Transforms.NormalizeMinMax(nameof(HealthPredictionMLInput.AvgReminderDelay)))
            .Append(context.Transforms.Concatenate("Features",
                nameof(HealthPredictionMLInput.SunlightIntensity),
                nameof(HealthPredictionMLInput.HumidityIntensity),
                nameof(HealthPredictionMLInput.Month),
                nameof(HealthPredictionMLInput.IsOutside),
                nameof(HealthPredictionMLInput.IsLowMaintenance),
                nameof(HealthPredictionMLInput.IsDroughtResistant),
                "FamilyEncoded", 
                "HardinessEncoded", 
                nameof(HealthPredictionMLInput.SunlightRequirements), 
                nameof(HealthPredictionMLInput.MoistureRequirements),
                nameof(HealthPredictionMLInput.Seasons),
                nameof(HealthPredictionMLInput.AvgReminderDelay)
                ))
            .Append(context.Regression.Trainers.FastTree(
                labelColumnName: "Label",
                featureColumnName: "Features",
                numberOfTrees: 200,
                numberOfLeaves: 8,
                minimumExampleCountPerLeaf: 20,
                learningRate: 0.05
            ));

        var validationResults = context.Regression.CrossValidate(
            data: dataView,
            estimator: pipeline,
            numberOfFolds: 5,
            labelColumnName: "Label"
        );

        var acc = validationResults.Average(f => f.Metrics.RSquared);
        var avgRMSE = validationResults.Average(f => f.Metrics.RootMeanSquaredError);

        Console.WriteLine($"R2 (accuracy): {Math.Round(acc, 2)}");
        Console.WriteLine($"RMSE (average error): {Math.Round(avgRMSE, 2)}");

        var model = pipeline.Fit(dataView);

        /*var predictions = model.Transform(split.TestSet);
        var metrics = context.Regression.Evaluate(predictions, labelColumnName: "HealthScore");

        Console.WriteLine("R2 (accuracy): " + Math.Round(metrics.RSquared, 2));
        Console.WriteLine("RMSE (average error): " + Math.Round(metrics.RootMeanSquaredError, 2));*/

        if (!Directory.Exists(Path.GetDirectoryName(path)))
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        context.Model.Save(model, dataView.Schema, path);
    }

    

    private float CalculateAdjustedHealthScore(HealthPredictionRecord log)
    {
        float baseScore = log.PlantStatusId switch
        {
            6 or 7 or 9 => 100f,     // flowering, fruiting, harvested 
            1 or 5 => 85f,      // healthy, growing
            8 => 75f,           // seedling
            11 => 70f,          // transplanted
            12 => 50f,          // dormant
            10 => 40f,          // stressed
            4 => 20f,           // wilting
            2 => 10f,           // sick
            3 => 0f,            // dead
            _ => 50f
        };

        float adjustment = 0f;

        float minIdealSun = 5f;
        float maxIdealSun = 1f;

        foreach (var sun in log.SunlightList)
        {
            switch (sun.Id)
            {
                case 1: //full sun
                    minIdealSun = Math.Min(minIdealSun, 4f);
                    maxIdealSun = Math.Max(maxIdealSun, 5f); 
                    break; 
                case 2: //partial shade 
                    minIdealSun = Math.Min(minIdealSun, 2f);
                    maxIdealSun = Math.Max(maxIdealSun, 4f); 
                    break; 
                case 3: //full shade
                    minIdealSun = Math.Min(minIdealSun, 1f);
                    maxIdealSun = Math.Max(maxIdealSun, 2f); 
                    break;
            }
        }

        if (log.SunlightIntensity < minIdealSun) 
            adjustment -= 15f;
        else if (log.SunlightIntensity > maxIdealSun) 
            adjustment -= 10f;
        else 
            adjustment += 5f;


        var moistureIds = log.MoistureList.Select(m => m.Id).ToList();
        float minIdealMoist = 5f;
        float maxIdealMoist = 1f;

        foreach (var moisture in log.MoistureList)
        {
            switch (moisture.Id)
            {
                case 1: //well–drained
                    minIdealMoist = Math.Min(minIdealMoist, 1f);
                    maxIdealMoist = Math.Max(maxIdealMoist, 2f);
                    break;
                case 2: //poorly–drained
                    minIdealMoist = Math.Min(minIdealMoist, 4f);
                    maxIdealMoist = Math.Max(maxIdealMoist, 5f);
                    break;
                case 3: //moist but well–drained
                    minIdealMoist = Math.Min(minIdealMoist, 2f);
                    maxIdealMoist = Math.Max(maxIdealMoist, 4f);
                    break;
            }
        }

        if (log.HumidityIntensity < minIdealMoist) 
            adjustment -= 15f;
        else if (log.HumidityIntensity > maxIdealMoist) 
            adjustment -= 10f;
        else 
            adjustment += 5f;

        return Math.Clamp(baseScore + adjustment, 0f, 100f);
    }

    public async Task<float> PredictHealthScore(HealthPredictionMLInput input)
    {
        if (!File.Exists(path))
        {
            await TrainModelAsync();
            if (!File.Exists(path)) return 0f;
        }
        var context = new MLContext();

        if (_trainedModel == null)
        {
            _trainedModel = context.Model.Load(path, out _);
        }

        var predictionEngine = context.Model.CreatePredictionEngine<HealthPredictionMLInput, HealthMLPrediction>(_trainedModel);
        
        var prediction = predictionEngine.Predict(input);
        return prediction.PredictedHealthScore;
    }

    public async Task<List<float>> PredictHealthScoresBatch(List<HealthPredictionMLInput> inputs)
    {
        if (!File.Exists(path))
        {
            await TrainModelAsync();
            if (!File.Exists(path)) return inputs.Select(_ => 0f).ToList();
        }

        var context = new MLContext();

        if (_trainedModel == null)
        {
            _trainedModel = context.Model.Load(path, out _);
        }

        IDataView inputDataView = context.Data.LoadFromEnumerable(inputs);
        var predictions = _trainedModel.Transform(inputDataView);

        var scores = context.Data.CreateEnumerable<HealthMLPrediction>(predictions, reuseRowObject: false)
            .Select(p => p.PredictedHealthScore)
            .ToList();

        return scores;
    }

}
