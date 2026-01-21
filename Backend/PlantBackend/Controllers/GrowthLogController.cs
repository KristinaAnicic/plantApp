using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.GrowthLog;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/log")]
[ApiController]
[Authorize]
public class GrowthLogController(IGrowthLogService growthLogService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await growthLogService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("planted")]
    public async Task<IActionResult> GetAllByPlanted([FromQuery] int plantedId)
    {
        var result = await growthLogService.GetAllByPlantedIdAsync(plantedId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await growthLogService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertGrowthLogDto dto)
    {
        await growthLogService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertGrowthLogDto dto)
    {
        await growthLogService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await growthLogService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromBody] List<string> urls)
    {
        await growthLogService.AddImages(id, urls);
        return NoContent();
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        await growthLogService.RemoveImageById(id, imageId);
        return NoContent();
    }
}
