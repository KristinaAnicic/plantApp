using Microsoft.EntityFrameworkCore;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.User;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
namespace PlantApp.Domain.Services.Data;

public class UserService(
    IRepository<User> repository
) : IUserService
{
    public async Task<List<User>> GetAllUsers()
    {
        return await repository.GetAllAsync();
    }

    public async Task<UserGetDto?> GetUser(int id)
    {
        var user = await repository.GetByIdAsync(id);
        return user?.MapUserToUserGetDto();
    }

    public async Task AddUser(AddUserDto dto)
    {
        var existingEmail = await repository.ExistsAsync(u => EF.Functions.ILike(u.Email, dto.Email));
        if (existingEmail) 
            throw new ArgumentException("Email already exists");

        var existingUsername = await repository.ExistsAsync(u => EF.Functions.ILike(u.Username, dto.Username));
        if (existingUsername)
            throw new ArgumentException("Username already exists");

        var plant = dto.MapAddUserDtoToUser();
        plant.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 13);

        await repository.AddAsync(plant);
    }

    public async Task UpdateUser(int id, UpdateUserDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (id != dto.Id)
            throw new ArgumentException("DTO ID does not match the provided Id parameter.");

        var existingUser = await repository.GetByIdAsync(id);

        if (existingUser == null)
            throw new ArgumentException("User with the provided Id does not exist.");

        dto.MapUpdateUserDtoToUser(existingUser);
        
        await repository.UpdateAsync(existingUser);
    }

    public async Task DeleteUser(int id)
    {
        var user = await repository.GetByIdAsync(id);

        if (user == null)
            throw new ArgumentException("User with the provided Id does not exist.");

        await repository.DeleteAsync(user);
    }
}
