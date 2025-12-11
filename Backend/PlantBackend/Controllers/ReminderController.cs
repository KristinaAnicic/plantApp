using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Reminder;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/reminder")]
[ApiController]
public class ReminderController(IReminderService reminderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await reminderService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await reminderService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UpsertReminderDto dto)
    {
        await reminderService.AddAsync(dto);
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpsertReminderDto dto)
    {
        await reminderService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpPut("{id}/delay")]
    public async Task<IActionResult> Delay(int id, [FromQuery] int delay)
    {
        await reminderService.DelayReminderAsync(id, delay);
        return NoContent();
    }

    [HttpPut("{id}/done")]
    public async Task<IActionResult> Done(int id, [FromQuery] DateTime? date)
    {
        await reminderService.ReminderDoneAsync(id, date);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await reminderService.DeleteAsync(id);
        return NoContent();
    }
}
