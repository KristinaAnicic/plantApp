using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Data;

namespace PlantBackend.Controllers;

[Route("api/rating")]
[Authorize]
public class UserRatingController(IUserRatingService userRatingService) : Controller
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAll(int id)
    {
        var result = await userRatingService.GetAllForUserIdAsync(id);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddUserRatingDto dto)
    {
        var error = await userRatingService.AddAsync(dto);
        if (error != null)
            return StatusCode(error.StatusCode, error);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRatingDto dto)
    {
        await userRatingService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await userRatingService.DeleteAsync(id);
        return NoContent();
    }
}
