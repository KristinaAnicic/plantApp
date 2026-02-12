using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.PlantGroup;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/group")]
[ApiController]
[Authorize]
public class PlantGroupController(IPlantGroupService plantgroupService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await plantgroupService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await plantgroupService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertPlantGroupDto dto)
    {
        await plantgroupService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertPlantGroupDto dto)
    {
        await plantgroupService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await plantgroupService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/plants")]
    public async Task<IActionResult> AddPlantsToGroup(int id, [FromBody] List<int> plantIds)
    {
        await plantgroupService.AddPlantsToGroupAsync(id, plantIds);
        return NoContent();
    }

    [HttpPost("{id}/plant/{plantId}")]
    public async Task<IActionResult> AddSinglePlantToGroup(int id, int plantId)
    {
        await plantgroupService.AddPlantToGroupAsync(id, plantId);
        return NoContent();
    }

    [HttpDelete("remove-plant/{plantId}")]
    public async Task<IActionResult> RemovePlantFromGroup(int plantId)
    {
        await plantgroupService.RemovePlantFromGroupAsync(plantId);
        return NoContent();
    }
}
