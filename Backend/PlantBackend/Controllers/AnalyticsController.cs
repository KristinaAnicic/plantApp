using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Interfaces;

namespace PlantBackend.Controllers;


[Route("api/analytics")]
[ApiController]
[Authorize]
public class AnalyticsController(IAnalyticsService service, IMLService mlService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAnalytics();
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("train")]
    public async Task<IActionResult> Train()
    {
        await mlService.TrainModelAsync();
        return Ok();
    }
}
