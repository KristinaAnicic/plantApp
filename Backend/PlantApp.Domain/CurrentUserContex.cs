namespace PlantApp.Domain;
using Microsoft.AspNetCore.Http;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Utils.Exceptions;
using System.Security.Claims;

public class CurrentUserContex(IHttpContextAccessor httpContextAccessor) : ICurrentUserContext
{
    public int? TryGetCurrentUserId()
    {
        var userIdStr = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr))
            return null;

        return int.Parse(userIdStr);
    }

    public int GetCurrentUserId()
    {
        var userIdStr = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr))
            throw new UnauthorizedException("get current user id");

        return int.Parse(userIdStr);
    }

    public int GetCurrentUserRoleId()
    {
        var roleIdStr = httpContextAccessor.HttpContext?.User.FindFirst("roleId")?.Value;
        if (string.IsNullOrEmpty(roleIdStr))
            throw new UnauthorizedException("get current user role id");

        return int.Parse(roleIdStr);
    }
}
