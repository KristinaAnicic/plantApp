namespace PlantApp.Domain.Interfaces;

public interface ICurrentUserContext
{
    public int GetCurrentUserId();
    public int GetCurrentUserRoleId();
}
