namespace PlantApp.Domain;
using Microsoft.AspNetCore.Http;
using PlantApp.Domain.Interfaces;
using System.Security.Claims;

public class CurrentUserContex(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public int GetCurrentUserId()
    {
        var userIdStr = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdStr!);
    }

    public int GetCurrentUserRoleId()
    {
        var roleIdStr = httpContextAccessor.HttpContext?.User.FindFirst("roleId")?.Value;
        return int.Parse(roleIdStr!);
    }
}
