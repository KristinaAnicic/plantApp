using PlantApp.Domain.Dtos.User;
using PlantBackend.ExceptionHandlers;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserService
{
    public Task<List<UserDto>> GetAllAsync();
    public Task<UserGetDto?> GetByIdAsync(int id);
    public Task<ErrorResponse?> AddAsync(AddUserDto dto, bool isSelfRegistration = false);
    public Task<ErrorResponse?> UpdateAsync(int id, UpdateUserDto dto);
    public Task DeleteAsync(int id);
}
