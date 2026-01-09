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
    private const string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] AddUserDto dto)
    {
        await userService.AddAsync(dto);
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto dto) 
    {
        var (token, response) = await authService.LoginUser(dto);
        SetRefreshTokenCookie(token);

        return Ok(response);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<TokenResponseDto>> RefreshToken()
    {
        if (!(Request.Cookies.TryGetValue(RefreshTokenCookieName, out var refreshToken)))
            return Unauthorized("Refresh token missing or invalid");

        try
        {
            var (token, response) = await authService.RefreshTokens(refreshToken);
            SetRefreshTokenCookie(token);

            return Ok(response);
        }

        catch
        {
            Response.Cookies.Delete(RefreshTokenCookieName);
            return Unauthorized("Invalid or expired refresh token");
        }
    }

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
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7),
            Path = "/api/auth"
        });
    }
}
