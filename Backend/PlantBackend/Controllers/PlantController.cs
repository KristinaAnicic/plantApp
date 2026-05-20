using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Plant;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Models.Interfaces;

namespace PlantBackend.Controllers;

[Route("api/plant")]
[ApiController]
public class PlantController(
    IPlantService plantService, 
    IImageService imageService,
    IPlantNetService plantNetService,
    IPlantDIseaseService plantDiseaseService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1)
    {
        var result = await plantService.GetAllAsync(page);
        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<IActionResult> GetAllFIltered([FromBody] FilterByDto filter, [FromQuery] int page = 1)
    {
        var result = await plantService.GetFilteredAsync(filter, page);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlantDto dto)
    {
        await plantService.AddAsync(dto);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromBody] List<string> urls)
    {
        await plantService.AddImages(id, urls);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlantDto dto)
    {
        var error = await plantService.UpdateAsync(id, dto);
        if (error != null)
            return StatusCode(error.StatusCode, error);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("unused-images")]
    public async Task<IActionResult> DeleteUnusedImages()
    {
        await imageService.RemoveUnusedImagesAsync();
        return NoContent();
    }

    [HttpPost("identify")]
    public async Task<IActionResult> IdentifyPlant([FromForm] List<IFormFile> images)
    {
        var result = await plantNetService.IdentifyPlantAsync(images);
        return Ok(result);
    }

    [HttpPost("disease")]
    public async Task<IActionResult> IdentifyDisease([FromForm] IFormFile image)
    {
        var result = await plantDiseaseService.PredictAsync(image);
        return Ok(result);
    }
}
