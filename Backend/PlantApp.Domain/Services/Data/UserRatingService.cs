using Microsoft.Extensions.Logging;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.PlantExchange;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Data;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;

namespace PlantApp.Domain.Services.Data;

public class UserRatingService(
    IRepository<UserRating> repository,  
    IRepository<User> userRepo,
    ICurrentUserContext userContext,
    ILogger<UserRatingService> logger
) : IUserRatingService
{
    private int CurrentUserId => userContext.GetCurrentUserId();
    private bool IsAdmin => userContext.GetCurrentUserRoleId() == 1;

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
        if (rating.Any() && !IsAdmin) 
            throw new InvalidOperationAppException("You have already rated this user. You can update the existing rating instead.",null, logger);

        var userExists = await userRepo.IdExistsAsync(dto.RatedUserId);
        if (!userExists) 
            throw new NotFoundException("User", dto.RatedUserId, logger);

        var newRating = dto.MapAddUserRatingDtoToUserRating();
        newRating.RaterId = CurrentUserId;

        await repository.AddAsync(newRating);

        logger.LogInformation("User {UserId} added a rating for user {RatedUserId}", CurrentUserId, dto.RatedUserId);
    }

    public async Task UpdateAsync(int id, UpdateUserRatingDto dto)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null) 
            throw new NotFoundException("User rating", id, logger);
        if (rating.RaterId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("update", "user rating", logger);     

        dto.MapUpdateUserRatingDtoToUserRating(rating);

        await repository.UpdateAsync(rating);

        logger.LogInformation("User {UserId} updated rating {RatingId}", CurrentUserId, id);
    }

    public async Task DeleteAsync(int id)
    {
        var rating = await repository.GetByIdAsync(id);

        if (rating == null) 
            throw new NotFoundException("User rating", id, logger);
        if (rating.RaterId != CurrentUserId && !IsAdmin) 
            throw new UnauthorizedException("delete", "user rating", logger);
        
        await repository.DeleteAsync(rating, false);

        logger.LogInformation("User {UserId} deleted rating {RatingId}", CurrentUserId, id);
    }
}
