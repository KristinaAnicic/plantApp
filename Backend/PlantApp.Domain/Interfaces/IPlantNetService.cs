using Microsoft.AspNetCore.Http;
using PlantApp.Domain.Dtos.PlantNet;

namespace PlantApp.Domain.Interfaces;

public interface IPlantNetService
{
    public Task<PlantNetResponse?> IdentifyPlantAsync(List<IFormFile> images);
}
