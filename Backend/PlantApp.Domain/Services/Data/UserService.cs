using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
namespace PlantApp.Domain.Services.Data;

public class UserService(
    IUserRepository repository,
    IRepository<Role> roleRepo
) : IUserService
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await repository.GetAllUsers();
        return users.Select(u => u.MapUserToUserDto()).ToList();
    }

    public async Task<UserGetDto?> GetByIdAsync(int id)
    {
        var user = await repository.GetUserById(id);
        if (user == null)
            throw new ArgumentException("User not found");
        return user?.MapUserToUserGetDto();
    }

    public async Task AddAsync(AddUserDto dto)
    {
        var existingEmail = await repository.ExistsAsync(u => EF.Functions.ILike(u.Email, dto.Email));
        if (existingEmail) 
            throw new ArgumentException("Email already exists");

        var existingUsername = await repository.ExistsAsync(u => EF.Functions.ILike(u.Username, dto.Username));
        if (existingUsername)
            throw new ArgumentException("Username already exists");

        var role = await roleRepo.GetByIdAsync(dto.RoleId);
        if (role == null)
            throw new ArgumentException("Role not found");

        int currentUserRole = 3;
        if (currentUserRole != 1 && dto.RoleId != 3)
            dto.RoleId = 3;

        var plant = dto.MapAddUserDtoToUser();
        plant.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 13);

        await repository.AddAsync(plant);
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (id != dto.Id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingUser = await repository.GetByIdAsync(id);

        if (existingUser == null)
            throw new ArgumentException("User with the provided Id does not exist.");

        int currentUserRole = 3;
        if (currentUserRole != 1 && dto.RoleId != 3)
            dto.RoleId = 3;

        dto.MapUpdateUserDtoToUser(existingUser);
        
        await repository.UpdateAsync(existingUser);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);

        if (user == null)
            throw new ArgumentException("User with the provided Id does not exist.");

        await repository.DeleteAsync(user);
    }
}
