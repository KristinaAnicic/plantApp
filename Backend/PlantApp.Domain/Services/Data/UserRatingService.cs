using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;

namespace PlantApp.Domain.Services.Data;

public class UserRatingService(
    IRepository<UserRating> repository,  
    IRepository<User> userRepo,
    ICurrentUserContext userContext,
    ILogger<UserRatingService> logger
) : IUserRatingService
{
    private int CurrentUserId => userContext.GetCurrentUserId();

    public async Task<List<UserRatingDto>> GetAllForUserIdAsync(int ratedUserId)
    {
        var ratings = await repository.GetAllByKeyAsync(u => u.RatedId == ratedUserId, true);

        ratings = ratings
            .OrderByDescending(o => o.RaterId == CurrentUserId)
            .ThenBy(o => o.CreatedAt)
            .ToList();

        logger.LogInformation("Retrieved {Count} ratings for user {RatedUserId}", ratings.Count, ratedUserId);


        return ratings.Select(r => r.MapUserRatingToUserRatingDto()).ToList();
    }

    public async Task AddAsync(AddUserRatingDto dto)
    {
        var rating = await repository.GetAllByKeyAsync(u => u.RatedId == dto.RatedUserId && u.RaterId == CurrentUserId);
        if (rating.Any())
        {
            logger.LogWarning("User {UserId} attempted to add duplicate rating for user {RatedUserId}", CurrentUserId, dto.RatedUserId);
            throw new InvalidOperationException("You have already rated this user. You can update the existing rating instead.");
        }

        var userExists = await userRepo.IdExistsAsync(dto.RatedUserId);
        if (!userExists)
        {
            logger.LogWarning("Rated user {RatedUserId} not found when user {UserId} attempted to rate", dto.RatedUserId, CurrentUserId);
            throw new KeyNotFoundException("The user you are trying to rate does not exist.");
        }

        var newRating = dto.MapAddUserRatingDtoToUserRating();
        newRating.RaterId = CurrentUserId;

        await repository.AddAsync(newRating);

        logger.LogInformation("User {UserId} added a rating for user {RatedUserId}", CurrentUserId, dto.RatedUserId);
    }

    public async Task UpdateAsync(int id, UpdateUserRatingDto dto)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null)
        {
            logger.LogWarning("Rating {RatingId} not found for update by user {UserId}", id, CurrentUserId);
            throw new KeyNotFoundException("The rating you are trying to update does not exist.");
        }

        if (rating.RaterId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to update rating {RatingId} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to update this rating.");
        }

        dto.MapUpdateUserRatingDtoToUserRating(rating);

        await repository.UpdateAsync(rating);

        logger.LogInformation("User {UserId} updated rating {RatingId}", CurrentUserId, id);
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null)
        {
            logger.LogWarning("Rating {RatingId} not found for deletion by user {UserId}", id, CurrentUserId);
            throw new KeyNotFoundException("The rating you are trying to delete does not exist.");
        }

        if (rating.RaterId != CurrentUserId)
        {
            logger.LogWarning("User {UserId} attempted to delete rating {RatingId} without permission", CurrentUserId, id);
            throw new UnauthorizedAccessException("You are not authorized to delete this rating.");
        }

        await repository.DeleteAsync(rating, false);

        logger.LogInformation("User {UserId} deleted rating {RatingId}", CurrentUserId, id);
    }
}
