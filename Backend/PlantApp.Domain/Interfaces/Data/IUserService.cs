using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.User;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserService
{
    public Task<List<User>> GetAllAsync();
    public Task<UserGetDto?> GetByIdAsync(int id);
    public Task AddAsync(AddUserDto dto);
    public Task UpdateAsync(int id, UpdateUserDto dto);
    public Task DeleteAsync(int id);
}
