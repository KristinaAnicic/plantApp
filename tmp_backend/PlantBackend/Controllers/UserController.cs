using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PlantBackend.Controllers;

[Route("api/user")]
[ApiController]
public class UserController(IUserService userService) : Controller
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await userService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await userService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AddUserDto dto)
    {
        var error = await userService.AddAsync(dto, isSelfRegistration: false);
        if (error != null)
            return StatusCode(error.StatusCode, error);

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var error = await userService.UpdateAsync(id, dto);
        if (error != null)
            return StatusCode(error.StatusCode, error);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await userService.DeleteAsync(id);
        return NoContent();
    }
}
