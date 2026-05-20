using Microsoft.AspNetCore.Http;
using PlantApp.Domain.Dtos.DiseasePrediction;

namespace PlantApp.Domain.Interfaces;

public interface IPlantDIseaseService
{
    public Task<DiseasePredictionResponse> PredictAsync(IFormFile file);
}
