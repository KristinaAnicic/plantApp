using PlantApp.Data.Models;

namespace PlantApp.Domain.Interfaces.Repository;

public interface IUserRepository : IRepository<User>
{
    public Task<User?> GetUserById(int id);
    public Task<List<User>> GetAllUsers();
}
