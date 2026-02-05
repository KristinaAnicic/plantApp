using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Domain.Dtos.Authentication;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

[Route("api/auth")]
[ApiController]
public class AuthController(
    IUserService userService,
    IAuthService authService
    
    ) : Controller
{
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] AddUserDto dto)
    {
        var error = await userService.AddAsync(dto, isSelfRegistration: true);
        if (error != null)
            return StatusCode(error.StatusCode, error);

        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto) 
    {
        var (token, error) = await authService.LoginUser(dto);
        if (error != null)
        {
            return StatusCode(error.StatusCode, error);
        }
        SetRefreshTokenCookie(token!.RefreshToken!);

        return Ok(token);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken()
    {
        if (!(Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken)))
            return Unauthorized("Refresh token missing or invalid");

        try
        {
            var response = await authService.RefreshTokens(refreshToken);
            SetRefreshTokenCookie(response.RefreshToken);

            return Ok(response);
        }

        catch
        {
            Response.Cookies.Delete(RefreshTokenCookieName);
            return Unauthorized("Invalid or expired refresh token");
        }
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<ActionResult<TokenResponseDto>> Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken))
        {
            await authService.Logout(refreshToken);
        }

        Response.Cookies.Delete(RefreshTokenCookieName);
        return NoContent();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, new CookieOptions()
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth"
        });
    }
}
