using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Interfaces;

namespace PlantBackend.Controllers;


[Route("api/analytics")]
[ApiController]
[Authorize]
public class AnalyticsController(
    IAnalyticsService service, 
    IMLHealthPredictionService mlService,
    IMLRecommendationService mlRecService
    ) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAnalytics();
        return Ok(result);
    }

    [HttpGet("{plantedId}")]
    public async Task<IActionResult> GetAnalyticsForPlanted(int plantedId)
    {
        var result = await service.GetPlantedAnalytics(plantedId);
        return Ok(result);
    }

    [HttpGet("group/{groupId}/{year}")]
    public async Task<IActionResult> GetAnalyticsForPlantGroup(int groupId, int? year = null)
    {
        var result = await service.GetPlantedGroupAnalytics(groupId, year);
        return Ok(result);
    }

    //[Authorize(Roles = "Admin")]
    [HttpPost("train/health_prediction")]
    public async Task<IActionResult> TrainHealthPrediction()
    {
        await mlService.TrainModelAsync();
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("train/plant_recommendation")]
    public async Task<IActionResult> TrainRecommendation()
    {
        await mlRecService.TrainModelAsync();
        return Ok();
    }
}
