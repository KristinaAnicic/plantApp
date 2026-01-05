using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Authentication;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;

[Route("api/auth")]
[ApiController]
public class AuthController(
    IUserService userService,
    IAuthService authService
    
    ) : Controller
{
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AddUserDto dto)
    {
        await userService.AddAsync(dto);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto) 
    { 
        var response = await authService.LoginUser(dto);
        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var response = await authService.RefreshTokens(dto);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<ActionResult<TokenResponseDto>> Logout([FromBody] RefreshTokenRequestDto dto)
    {
        await authService.Logout(dto);
        return NoContent();
    }
}
