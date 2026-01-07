using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Authentication;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using PlantApp.Domain.Utils;
using PlantApp.Domain.Utils.Exceptions;
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
    private static readonly int lifeSpanInMinutes = 15;
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
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateAndSaveRefreshTokenAsync(int userId)
    {
        var refreshToken = GenerateRefreshToken();
        await refreshTokenRepo.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = userId,
            ExpiryTime = DateTime.UtcNow.AddDays(7)
        });

        logger.LogInformation("Refresh token generated for UserId {UserId}", userId);
        return refreshToken;
    }

    private async Task<RefreshToken?> ValidateRefreshTokenAsync(int userId, string refreshToken)
    {
        var existingRefreshToken = await refreshTokenRepo.GetByKeyAsync(
            r => r.UserId == userId &&
                 r.Token == refreshToken &&
                 r.RevokedAt == null &&
                 r.ExpiryTime > DateTime.UtcNow
        );

        return existingRefreshToken;
    }

    public async Task<TokenResponseDto> RefreshTokens(RefreshTokenRequestDto request)
    {
        var refreshToken = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

        if (refreshToken == null)
            throw new InvalidOperationAppException(
                userMessage: "Refresh token is invalid or expired.",
                internalMessage: $"Failed refresh token attempt for UserId {request.UserId} with token '{request.RefreshToken}'",
                logger: logger
            );

        var user = await userRepo.GetByIdAsync(request.UserId);
        if (user == null) {
            throw new NotFoundException("User", request.UserId, logger);
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await refreshTokenRepo.UpdateAsync(refreshToken);

        logger.LogInformation("Refresh token used and revoked for UserId {UserId}", request.UserId);
        return await CreateTokenResponse(user);
    }

    public async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id),
            User = user.MapUserToUserDto()
        };
    }

    public async Task<TokenResponseDto> LoginUser(LoginDto dto)
    {
        var user = await userRepo.GetByKeyAsync(u => 
            EF.Functions.ILike(u.Username.ToLower(), dto.UsernameOrEmail) || 
            EF.Functions.ILike(u.Email.ToLower(), dto.UsernameOrEmail), true);

        if (user == null || 
            !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.Password))
        {
            throw new InvalidOperationAppException(
            userMessage: "Username or password is incorrect.",
            internalMessage: $"Failed login attempt for UsernameOrEmail '{dto.UsernameOrEmail}'",
            logger: logger
        );
        }

        logger.LogInformation("User {UserId} successfully logged in", user.Id);
        return await CreateTokenResponse(user);
    }

    public async Task Logout(RefreshTokenRequestDto dto)
    {
        var token = await refreshTokenRepo.GetByKeyAsync(
            r => r.UserId == dto.UserId &&
                 r.Token == dto.RefreshToken &&
                 r.RevokedAt == null &&
                 r.ExpiryTime > DateTime.UtcNow
        );

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await refreshTokenRepo.UpdateAsync(token);
            logger.LogInformation("Refresh token revoked for UserId {UserId}", dto.UserId);
        }
        else
        {
            logger.LogWarning("Attempt to revoke non-existing or already revoked refresh token for UserId {UserId}", dto.UserId);
        }
    }
}
