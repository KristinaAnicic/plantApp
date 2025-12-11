using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserRatingService
{
    public Task<List<UserRatingDto>> GetAllForUserIdAsync(int ratedUserId);
    public Task AddAsync(AddUserRatingDto dto);
    public Task UpdateAsync(int id, UpdateUserRatingDto dto);
    public Task DeleteAsync(int id);
}
