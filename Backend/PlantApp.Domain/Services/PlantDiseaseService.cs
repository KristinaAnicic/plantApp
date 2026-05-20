using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PlantApp.Domain.Dtos.DiseasePrediction;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Utils.Exceptions;
using System.Net.Http.Json;

namespace PlantApp.Domain.Services;

public class PlantDiseaseService(
    IHttpClientFactory httpClientFactory,
    ILogger<PlantDiseaseService> logger) : IPlantDIseaseService
{
    public async Task<DiseasePredictionResponse> PredictAsync(IFormFile file)
    {
        var client = httpClientFactory.CreateClient("MlApi");

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(file.OpenReadStream());
        content.Add(streamContent, "file", file.FileName);

        var response = await client.PostAsync("/predict", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<DiseasePredictionResponse>();

        if (result == null)
            throw new InvalidOperationAppException("An error occurred while identifying the plant disease. Please try again.", null, logger);

        return result;
    }
}
