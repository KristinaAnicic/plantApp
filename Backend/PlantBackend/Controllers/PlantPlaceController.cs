using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.PlantPlace;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/place")]
[ApiController]
[Authorize]
public class PlantPlaceController(IPlantPlaceService plantPlaceService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int userId)
    {
        var result = await plantPlaceService.GetAllAsync(userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantPlaceService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlaceDto dto)
    {
        await plantPlaceService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlaceDto dto)
    {
        await plantPlaceService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantPlaceService.DeleteAsync(id);
        return NoContent();
    }
}
