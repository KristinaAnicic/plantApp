using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/plant")]
[ApiController]
public class PlantController(IPlantService plantService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await plantService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAllFIltered([FromBody] FilterByDto filter)
    {
        var result = await plantService.GetFilteredAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlantDto dto)
    {
        await plantService.AddAsync(dto);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromBody] List<string> urls)
    {
        await plantService.AddImages(id, urls);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlantDto dto)
    {
        await plantService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantService.DeleteAsync(id);
        return NoContent();
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        await plantService.RemoveImageById(id, imageId);
        return NoContent();
    }

    [HttpGet("multi-reference")]
    public async Task<IActionResult> GetAllMultiReferences()
    {
        var result = await plantService.GetMultiReferenceDataAsync();
        return Ok(result);
    }

    [HttpGet("single-reference")]
    public async Task<IActionResult> GetAllSingleReferenceData()
    {
        var result = await plantService.GetSinglePlantReferenceDataAsync();
        return Ok(result);
    }
}
