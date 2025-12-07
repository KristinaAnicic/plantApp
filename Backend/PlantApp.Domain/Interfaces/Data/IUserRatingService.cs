using PlantApp.Domain.Dtos.PlantExchange;

namespace PlantApp.Domain.Interfaces.Data;

public interface IUserRatingService
{
    public Task<List<UserRatingDto>> GetRatingsForUserId(int ratedUserId);
    public Task AddRating(AddUserRatingDto dto);
    public Task UpdateRating(int id, UpdateUserRatingDto dto);
    public Task DeleteRating(int id);
}
