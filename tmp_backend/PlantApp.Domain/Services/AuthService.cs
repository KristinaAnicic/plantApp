using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PlantApp.Domain.Dtos.Authentication;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Models;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
using PlantBackend.ExceptionHandlers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PlantApp.Domain.Services;

public class AuthService(
    IConfiguration config,
    IUserRepository userRepo,
    IRepository<RefreshToken> refreshTokenRepo,
    ILogger<AuthService> logger
    ) : IAuthService
{
    private readonly string _key = config["Jwt:Key"]!;
    private readonly string _issuer = config["Jwt:Issuer"]!;
    private readonly string _audience = config["Jwt:Audience"]!;
    private readonly int lifeSpanInMinutes = 15;
    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role!.Name),
            new Claim("roleId", user.RoleId.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer : _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(lifeSpanInMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private static string HashToken(string token)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(int userId)
    {
        int maxRetries = 3;

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            var refreshToken = GenerateRefreshToken();
            var tokenHash = HashToken(refreshToken);

            var tokenEntity = new RefreshToken
            {
                Token = tokenHash,
                UserId = userId,
                ExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            try
            {
                await refreshTokenRepo.AddAsync(tokenEntity);
                logger.LogInformation("Refresh token generated for UserId {UserId}", userId);
                return refreshToken;
            }
            catch (DbUpdateException ex) {
                logger.LogWarning(ex, "Refresh token collision detected, retrying (attempt {Attempt})", attempt + 1);

                if (attempt == maxRetries - 1)
                    throw new InvalidOperationAppException("Could not generate unique refresh token after multiple attempts", null, logger);
            }
            
        }

        throw new InvalidOperationAppException("Failed to generate refresh token", null, logger);
    }

    private async Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var tokenHash = HashToken(refreshToken);

        var existingRefreshToken = await refreshTokenRepo.GetByKeyAsync(
            r => r.Token == tokenHash &&
                 r.RevokedAt == null &&
                 r.ExpiryTime > DateTime.UtcNow
        );

        return existingRefreshToken;
    }

    public async Task<TokenResponseDto> RefreshTokens(string token)
    {
        var refreshToken = await ValidateRefreshTokenAsync(token);

        if (refreshToken == null)
            throw new InvalidOperationAppException(
                userMessage: "Refresh token is invalid or expired.",
                internalMessage: $"Failed refresh token attempt with token '{token}'",
                logger: logger
            );

        var user = await userRepo.GetByIdAsync(refreshToken.UserId);
        if (user == null) {
            throw new NotFoundException("User", refreshToken.UserId, logger);
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await refreshTokenRepo.UpdateAsync(refreshToken);

        logger.LogInformation("Refresh token used and revoked for UserId {UserId}", refreshToken.UserId);
        return await CreateTokenResponse(user);
    }

    public async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        var refreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id);
        return new TokenResponseDto
        {
            AccessToken = GenerateToken(user),
            User = user.MapUserToUserDto(),
            RefreshToken = refreshToken
        };
    }

    public async Task<(TokenResponseDto?, ErrorResponse?)> LoginUser(LoginDto dto)
    {
        var user = await userRepo.GetByKeyAsync(u => 
            EF.Functions.ILike(u.Username, dto.UsernameOrEmail) || 
            EF.Functions.ILike(u.Email, dto.UsernameOrEmail), true);

        if (user == null || 
            !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.Password))
        {
            logger.LogWarning("Failed login attempt for '{UsernameOrEmail}'", dto.UsernameOrEmail);

            return (null, new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Error = "Username or password is incorrect."
            });
        }

        logger.LogInformation("User {UserId} successfully logged in", user.Id);
        return (await CreateTokenResponse(user), null);
    }

    public async Task Logout(string refreshToken)
    {
        var token = await ValidateRefreshTokenAsync(refreshToken);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await refreshTokenRepo.UpdateAsync(token);
            logger.LogInformation("Refresh token revoked for UserId {UserId}", token.UserId);
        }
        else
        {
            logger.LogWarning("Attempt to revoke non-existing or already revoked refresh token");
        }
    }
}
