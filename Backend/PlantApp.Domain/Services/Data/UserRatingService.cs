using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class UserRatingService(
    IRepository<UserRating> repository,  
    IRepository<User> userRepo  
) : IUserRatingService
{
    public int currentUser = 0;

    public async Task<List<UserRatingDto>> GetAllForUserIdAsync(int ratedUserId)
    {
        var ratings = await repository.GetAllByKeyAsync(u => u.RatedId == ratedUserId, true);

        ratings = ratings
            .OrderByDescending(o => o.RaterId == currentUser)
            .ThenBy(o => o.CreatedAt)
            .ToList();

        return ratings.Select(r => r.MapUserRatingToUserRatingDto()).ToList();
    }

    public async Task AddAsync(AddUserRatingDto dto)
    {
        var rating = await repository.GetAllByKeyAsync(u => u.RatedId == dto.RatedUserId && u.RaterId == currentUser);
        if (rating.Any()) {
            throw new ArgumentException("Cannot add new rating but you can change the existing one!");
        }

        var existingUser = await userRepo.IdExistsAsync(dto.RatedUserId);

        if (!existingUser)
            throw new ArgumentException("Rated user not found");

        var newRating = dto.MapAddUserRatingDtoToUserRating();
        newRating.RaterId = currentUser;

        await repository.AddAsync(newRating);
    }

    public async Task UpdateAsync(int id, UpdateUserRatingDto dto)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null) {
            throw new ArgumentException("Rating not found");
        }

        if (rating.RaterId != currentUser) {
            throw new UnauthorizedAccessException("Access denied");
        }

        dto.MapUpdateUserRatingDtoToUserRating(rating);

        await repository.UpdateAsync(rating);
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null)
        {
            throw new ArgumentException("Rating not found");
        }

        if (rating.RaterId != currentUser)
        {
            throw new UnauthorizedAccessException("Access denied");
        }

        await repository.DeleteAsync(rating, false);
    }
}
