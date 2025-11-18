using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.User;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserService
{
    public Task<List<User>> GetAllUsers();
    public Task<UserGetDto?> GetUser(int id);
    public Task AddUser(AddUserDto dto);
    public Task UpdateUser(int id, UpdateUserDto dto);
    public Task DeleteUser(int id);
}
