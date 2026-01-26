using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Interfaces;

namespace PlantBackend.Controllers;


[Route("api/analytics")]
[ApiController]
[Authorize]
public class AnalyticsController(IAnalyticsService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await service.GetAnalytics();
        return Ok(result);
    }
}
