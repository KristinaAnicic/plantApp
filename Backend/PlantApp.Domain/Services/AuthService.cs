using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlantApp.Data.Models;
using PlantApp.Domain.Dtos.Authentication;
using PlantApp.Domain.Interfaces;
using PlantApp.Domain.Interfaces.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PlantApp.Domain.Services;

public class AuthService(
    IConfiguration config,
    IUserRepository userRepo,
    IRepository<RefreshToken> refreshTokenRepo
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
            throw new UnauthorizedAccessException("Invalid token"); 

        var user = await userRepo.GetByIdAsync(request.UserId);
        if (user == null) {
            throw new ArgumentException("User not found");
        }

        refreshToken.RevokedAt = DateTime.UtcNow;
        await refreshTokenRepo.UpdateAsync(refreshToken);

        return await CreateTokenResponse(user);
    }

    public async Task<TokenResponseDto> CreateTokenResponse(User user)
    {
        return new TokenResponseDto
        {
            AccessToken = GenerateToken(user),
            RefreshToken = await GenerateAndSaveRefreshTokenAsync(user.Id)
        };
    }

    public async Task<TokenResponseDto> LoginUser(LoginDto dto)
    {
        var user = await userRepo.GetByKeyAsync(u => 
            u.Username == dto.UsernameOrEmail || 
            u.Email == dto.UsernameOrEmail, true);

        if (user == null || 
            !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.Password))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

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
        }
    }
}
