using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Services.Data;

namespace PlantBackend.Controllers;

[Route("api/exchange")]
[ApiController]
[Authorize]
public class PlantExchangeController(IPlantExchangeService plantExchangeService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1)
    {
        var result = await plantExchangeService.GetActiveAsync(page);
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> GetAll([FromBody] PlantExchangeFilterDto filter, [FromQuery] int page = 1)
    {
        var result = await plantExchangeService.GetActiveFilteredAsync(filter, page);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantExchangeService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlantExchangeDto dto)
    {
        await plantExchangeService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlantExchangeDto dto)
    {
        await plantExchangeService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantExchangeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, [FromBody] List<string> urls)
    {
        await plantExchangeService.AddImages(id, urls);
        return NoContent();
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int id, int imageId)
    {
        var deletedUrl = await plantExchangeService.RemoveImageById(id, imageId);
        if (deletedUrl != null)
        {
            return Ok(new
            {
                imageDeleted = true,
                deletedUrl
            });
        }

        return NoContent();
    }
}
