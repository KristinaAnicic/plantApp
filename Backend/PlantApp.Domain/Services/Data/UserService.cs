using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
namespace PlantApp.Domain.Services.Data;

public class UserService(
    IUserRepository repository,
    IRepository<Role> roleRepo,
    ICurrentUserContext userContext,
    ILogger<UserService> logger
) : IUserService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private int CurrentUserRoleId => userContext.GetCurrentUserRoleId();

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await repository.GetAllUsers();
        logger.LogInformation("Retrieved {Count} users for current user {UserId}", users.Count, CurrentUserId);
        return users.Select(u => u.MapUserToUserDto()).ToList();
    }

    public async Task<UserGetDto?> GetByIdAsync(int id)
    {
        var user = await repository.GetUserById(id);
        if (user == null)
        {
            logger.LogWarning("User {UserId} not found", id);
            throw new KeyNotFoundException("The requested user does not exist.");
        }

        if (id != CurrentUserId && CurrentUserRoleId != 3)
        {
            logger.LogWarning("User {UserId} attempted to access information of {id} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to access information to this user.");
        }

        return user?.MapUserToUserGetDto();
    }

    public async Task AddAsync(AddUserDto dto)
    {
        var existingEmail = await repository.ExistsAsync(u => EF.Functions.ILike(u.Email, dto.Email));
        if (existingEmail)
        {
            logger.LogWarning("Attempt to add user with existing email: {Email}", dto.Email);
            throw new InvalidOperationException("This email is already registered.");
        }

        var existingUsername = await repository.ExistsAsync(u => EF.Functions.ILike(u.Username, dto.Username));
        if (existingUsername)
        {
            logger.LogWarning("Attempt to add user with existing username: {Username}", dto.Username);
            throw new InvalidOperationException("This username is already taken.");
        }

        var role = await roleRepo.GetByIdAsync(dto.RoleId);
        if (role == null)
        {
            logger.LogWarning("Role {RoleId} not found when adding new user", dto.RoleId);
            throw new KeyNotFoundException("The specified role does not exist.");
        }

        if (CurrentUserRoleId != 1 && dto.RoleId != 3)
        {
            dto.RoleId = 3;
            logger.LogInformation("Non-admin user {UserId} attempted to assign role {RoleId}, defaulted to regular user", CurrentUserId, dto.RoleId);
        }

        var user = dto.MapAddUserDtoToUser();
        user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 13);

        await repository.AddAsync(user);

        logger.LogInformation("User {UserId} added new user {NewUserEmail}", CurrentUserId, dto.Email);
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (id != dto.Id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingUser = await repository.GetByIdAsync(id);
        if (existingUser == null)
        {
            logger.LogWarning("Attempted update of non-existent user {UserId}", id);
            throw new KeyNotFoundException("The user you are trying to update does not exist.");
        }

        if (CurrentUserRoleId != 1 && dto.RoleId != 3)
        {
            dto.RoleId = 3;
            logger.LogInformation("Non-admin user {UserId} attempted to assign role {RoleId} on update, defaulted to regular user", CurrentUserId, dto.RoleId);
        }

        if (id != CurrentUserId && CurrentUserRoleId != 3)
        {
            logger.LogWarning("User {UserId} attempted to update user {UpdatedUserId} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to update this user.");
        }

        dto.MapUpdateUserDtoToUser(existingUser);
        
        await repository.UpdateAsync(existingUser);
        logger.LogInformation("User {UserId} updated user {UpdatedUserId}", CurrentUserId, id);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);

        if (user == null)
        {
            logger.LogWarning("Attempted delete of non-existent user {UserId}", id);
            throw new KeyNotFoundException("The user you are trying to delete does not exist.");
        }

        if (user.Id != CurrentUserId && CurrentUserRoleId != 3)
        {
            logger.LogWarning("User {UserId} attempted to delete user {DeletedUserId} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to delete this user.");
        }

        await repository.DeleteAsync(user);

        logger.LogInformation("User {UserId} deleted user {DeletedUserId}", CurrentUserId, id);
    }
}
