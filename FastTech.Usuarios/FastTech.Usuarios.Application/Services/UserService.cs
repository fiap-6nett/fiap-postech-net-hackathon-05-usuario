using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastTech.Usuarios.Application.Interfaces;
using FastTech.Usuarios.Application.Settings;
using FastTech.Usuarios.Domain.Dtos;
using FastTech.Usuarios.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FastTech.Usuarios.Application.Services;

public class UserService : IUserService
{
    private readonly IdentitySettings _identitySettings;

    public UserService(IOptions<IdentitySettings> identitySettings)
    {
        _identitySettings = identitySettings.Value;
    }

    private TokenPairDto GenerateTokenJwt(Guid id, bool isActive, UserRole role)
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
        var refreshToken = GenerateRefreshTokenJwt(id);

        return new TokenPairDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expires
        };
    }

    /// <summary>
    ///     Generates a refresh JWT token for the user.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private string GenerateRefreshTokenJwt(Guid id)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identitySettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = DateTime.UtcNow.AddMinutes(_identitySettings.AccessRefreshTokenMinutes);

        var jwt = new JwtSecurityToken(
            _identitySettings.Issuer,
            _identitySettings.Audience,
            claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}