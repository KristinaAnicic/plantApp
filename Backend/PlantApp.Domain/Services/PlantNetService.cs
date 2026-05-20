using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PlantApp.Domain.Dtos.PlantNet;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Utils.Exceptions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PlantApp.Domain.Services;

public class PlantNetService(
    HttpClient httpClient,
    IConfiguration config,
    ILogger<PlantNetService> logger
) : IPlantNetService
{
    public async Task<PlantNetResponse?> IdentifyPlantAsync(List<IFormFile> images)
    {
        var apiKey = config["PlantNet:ApiKey"];
        var baseUrl = config["PlantNet:BaseUrl"];

        var maxImages = images.Take(5).ToList();
        if (!images.Any())
            return null;

        using var content = new MultipartFormDataContent();

        foreach(var image in maxImages)
        {
            var stream = image.OpenReadStream();
            var streamContent = new StreamContent(stream);
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            content.Add(streamContent, "images", image.FileName);
        }

        var response = await httpClient.PostAsync($"{baseUrl}?api-key={apiKey}", content);

        if (!response.IsSuccessStatusCode)
        {
            var jsonError = await response.Content.ReadAsStringAsync();
            try
            {
                var errorObj = JsonSerializer.Deserialize<PlantNetErrorResponse>(jsonError,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var errorMessage = errorObj?.Message ?? "An unknown error occurred.";

                throw new InvalidOperationAppException(errorMessage, null, logger);
            }
            catch
            {
                throw new InvalidOperationAppException("An error occurred while identifying the plant. Please try again.", jsonError, logger);
            }
        }
            
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var responseSerialized = JsonSerializer.Deserialize<PlantNetResponse>(json, options);

        return responseSerialized;
    }
}
