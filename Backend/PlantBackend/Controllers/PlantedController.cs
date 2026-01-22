using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Planted;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/planted")]
[ApiController]
[Authorize]
public class PlantedController(IPlantedService plantedService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? userId)
    {
        var result = await plantedService.GetAllByUserIdAsync(userId);
        return Ok(result);
    }

    [HttpGet("grouped")]
    public async Task<IActionResult> GetAllGroupedByPlace([FromQuery] int? userId)
    {
        var result = await plantedService.GetAllByUserIdGroupedByPlaceAsync(userId);
        return Ok(result);
    }

    [HttpGet("place/{id}")]
    public async Task<IActionResult> GetAllByPlace(int id)
    {
        var result = await plantedService.GetAllByPlaceIdAsync(id);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantedService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlantedDto dto)
    {
        await plantedService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlantedDto dto)
    {
        await plantedService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantedService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromBody] List<string> urls)
    {
        await plantedService.AddImages(id, urls);
        return NoContent();
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        await plantedService.RemoveImageById(id, imageId);
        return NoContent();
    }

    [HttpGet("references")]
    public async Task<IActionResult> GetReferences()
    {
        var result = await plantedService.GetReferences();
        return Ok(result);
    }
}
