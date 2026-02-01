using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantApp.Domain.Models;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
namespace PlantApp.Domain.Services.Data;

public class UserService(
    IUserRepository repository,
    IRepository<Role> roleRepo,
    ICurrentUserContext userContext,
    ILogger<UserService> logger
) : IUserService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;

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
            throw new NotFoundException("User", id, logger);
        if (id != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("access", "user", logger);

        return user?.MapUserToUserGetDto();
    }

    public async Task AddAsync(AddUserDto dto)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (dto.DateOfBirth > today.AddYears(-13))
            throw new InvalidOperationAppException("You must be at least 13 years old.");

        var existingEmail = await repository.ExistsAsync(u => EF.Functions.ILike(u.Email, dto.Email));
        if (existingEmail)
        {
            throw new InvalidOperationAppException(
                userMessage: "This email is already registered.",
                internalMessage: $"Attempt to add user with existing email: {dto.Email}",
                logger: logger
            );
        }

        var existingUsername = await repository.ExistsAsync(u => EF.Functions.ILike(u.Username, dto.Username));
        if (existingUsername)
        {
            throw new InvalidOperationAppException(
                userMessage: "This username is already taken.",
                internalMessage: $"Attempt to add user with existing username: {dto.Username}",
                logger: logger
            );
        }

        var role = await roleRepo.GetByIdAsync(dto.RoleId);
        if (role == null) 
            throw new NotFoundException("Role", dto.RoleId, logger);

        if (!IsAdmin && dto.RoleId != 3)
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
            throw new InvalidOperationAppException("User data is required.", logger: logger);

        if (id != dto.Id) 
            throw new DtoIdMismatchException("User", dto.Id, id, logger);

        var existingUser = await repository.GetByIdAsync(id);
        if (existingUser == null) 
            throw new NotFoundException("User", id, logger);

        if (!IsAdmin && dto.RoleId != 3)
        {
            dto.RoleId = 3;
            logger.LogInformation("Non-admin user {UserId} attempted to assign role {RoleId} on update, defaulted to regular user", CurrentUserId, dto.RoleId);
        }

        if (id != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "user", logger);

        dto.MapUpdateUserDtoToUser(existingUser);
        
        await repository.UpdateAsync(existingUser);
        logger.LogInformation("User {UserId} updated user {UpdatedUserId}", CurrentUserId, id);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);

        if (user == null) 
            throw new NotFoundException("User", id, logger);
        if (user.Id != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("delete", "user", logger);

        await repository.DeleteAsync(user);

        logger.LogInformation("User {UserId} deleted user {DeletedUserId}", CurrentUserId, id);
    }
}
