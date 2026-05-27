using PlantApp.Domain.Dtos.PlantExchange;
using PlantBackend.ExceptionHandlers;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserRatingService
{
    public Task<List<UserRatingGetDto>> GetAllForUserIdAsync(int ratedUserId);
    public Task<ErrorResponse?> AddAsync(AddUserRatingDto dto);
    public Task UpdateAsync(int id, UpdateUserRatingDto dto);
    public Task DeleteAsync(int id);
}
