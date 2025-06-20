using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastTech.Usuarios.Application.Interfaces;
using FastTech.Usuarios.Application.Settings;
using FastTech.Usuarios.Domain.Entities;
using FastTech.Usuarios.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastTech.Usuarios.Application.Services;

public class UserService : IUserService
{
    private readonly IdentitySettings _identitySettings;
    private readonly ILogger<UserService> _logger;

    public UserService(ILogger<UserService> logger, IOptions<IdentitySettings> identitySettings)
    {
        _identitySettings = identitySettings.Value;
        _logger = logger;
    }

    /// <summary>
    ///     Generates a JWT token for the user based on their name and password in base64 format.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="passwordBase64"></param>
    /// <param name="loginIdentifierType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TokenEntity> GenerateTokenAsync(string name, string passwordBase64, LoginIdentifierType loginIdentifierType)
    {
        try
        {
            var decodedPassword = Encoding.UTF8.GetString(Convert.FromBase64String(passwordBase64));
            if (name.Equals("admin@admin.com.br", StringComparison.OrdinalIgnoreCase) && decodedPassword == "admin123")
            {
                // Simulating an admin user
                var tokenPair = GenerateTokenJwt(Guid.NewGuid(), true, UserRole.Admin);
                return tokenPair;
            }

            // Caso as credenciais estejam incorretas
            throw new UnauthorizedAccessException("Invalid credentials.");
        }
        catch (Exception e)
        {
            var message = $"Error generating token for user {name}: {e.Message}";
            _logger.LogError(message);
            throw new Exception(message);
        }
    }

    public Task<UserEntity> RegisterUserAsync(string name, string cpf, string Email, string passwordBase64, string passwordHash, UserRole role)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    ///     Generates a JWT token pair (AccessToken and RefreshToken) for the user.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private TokenEntity GenerateTokenJwt(Guid id, bool isActive, UserRole role)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_identitySettings.AccessTokenMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identitySettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims do usuário
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, id.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim("IsActive", isActive.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Token JWT (AccessToken)
            var jwtToken = new JwtSecurityToken(
                _identitySettings.Issuer,
                _identitySettings.Audience,
                claims,
                now,
                expires,
                creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new TokenEntity
            {
                AccessToken = accessToken,
                ExpiresAt = expires
            };
        }
        catch (Exception e)
        {
            var message = e.Message;
            _logger.LogError(message);
            throw new Exception(message);
        }
    }
}