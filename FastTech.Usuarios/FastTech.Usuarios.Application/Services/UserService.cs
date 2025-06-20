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
    private readonly IUserCommandStore _commandStore;
    private readonly IdentitySettings _identitySettings;
    private readonly ILogger<UserService> _logger;

    public UserService(IOptions<IdentitySettings> identityOptions, ILogger<UserService> logger, IUserCommandStore commandStore)
    {
        _identitySettings = identityOptions.Value;
        _logger = logger;
        _commandStore = commandStore;
    }

    /// <summary>
    ///     Generates a JWT token for the user based on their name and password in base64 format.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="passwordBase64"></param>
    /// <param name="loginIdentifierType"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<TokenEntity> GenerateTokenAsync(string user, string passwordBase64, LoginIdentifierType loginIdentifierType)
    {
        try
        {
            var plainPassword = Encoding.UTF8.GetString(Convert.FromBase64String(passwordBase64));

            UserEntity? userEntity = null;

            if (loginIdentifierType == LoginIdentifierType.Email)
                userEntity = await _commandStore.GetUserByEmailAndPasswordAsync(user);
            else
                userEntity = await _commandStore.GetUserByCpfAndPasswordAsync(user);

            if (userEntity is null) throw new UnauthorizedAccessException("Invalid credentials.");
            var isMatch = BCrypt.Net.BCrypt.Verify(plainPassword, userEntity.PasswordHash);
            if (!isMatch) throw new UnauthorizedAccessException("Invalid credentials.");
            // Generate JWT token
            var token = GenerateTokenJwt(userEntity.Id, userEntity.Role);
            return token;
        }
        catch (Exception e)
        {
            var message = $"Error generating token for user {user}: {e.Message}";
            _logger.LogError(message);
            throw new Exception(message);
        }
    }

    /// <summary>
    ///     Registers a new user in the system with the provided details.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cpf"></param>
    /// <param name="email"></param>
    /// <param name="passwordBase64"></param>
    /// <param name="role"></param>
    /// <returns>UserEntity</returns>
    public async Task<UserEntity> RegisterUserAsync(string name, string cpf, string email, string passwordBase64, UserRole role)
    {
        try
        {
            var plainPassword = Encoding.UTF8.GetString(Convert.FromBase64String(passwordBase64));
            cpf = UserEntity.SomenteNumeros(cpf);
            name = name.ToUpper();
            email = email.ToUpper();

            if (!UserEntity.IsValidCpf(cpf))
                throw new ArgumentException("Invalid CPF format.");

            var userExists = _commandStore.ExistsByEmailOrCpfAsync(email, cpf).GetAwaiter().GetResult();

            if (userExists)
            {
                var message = $"User with CPF {cpf} or Email {email} already exists.";
                _logger.LogError(message);
                throw new ArgumentException(message);
            }

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                Cpf = cpf,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword),
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _commandStore.CreateUserAsync(user);

            return user;
        }
        catch (Exception e)
        {
            var message = $"Error registering user {name}: {e.Message}";
            _logger.LogError(message);
            throw new Exception(message);
        }
    }


    /// <summary>
    ///     Generates a JWT token for the user based on their ID and role.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    public TokenEntity GenerateTokenJwt(Guid id, UserRole role)
    {
        try
        {
            var now = DateTime.UtcNow;
            var expires = now.AddMinutes(_identitySettings.AccessTokenMinutes);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_identitySettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                new Claim(ClaimTypes.Role, role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

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
            var message = $"Error generating JWT token for user {id}: {e.Message}";
            _logger.LogError(message);
            throw new Exception(message);
        }
    }

    /// <summary>
    ///     Retrieves a user by their ID, ensuring the requesting user has permission to access the data.
    /// </summary>
    /// <param name="targetUserId"></param>
    /// <param name="requestingUserId"></param>
    /// <param name="requestingUserRole"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="Exception"></exception>
    public Task<UserEntity?> GetUserByIdAsync(Guid targetUserId, string requestingUserId, string requestingUserRole)
    {
        try
        {
            var requestingGuidUserId = Guid.Parse(requestingUserId);
            var equals = targetUserId.Equals(requestingGuidUserId);

            if (!equals)
                if (requestingUserRole != UserRole.Admin.ToString() && requestingUserRole != UserRole.Manager.ToString())
                    throw new UnauthorizedAccessException("You do not have permission to access this user data.");

            var user = _commandStore.GetUserByIdAsync(targetUserId);
            return user;
        }
        catch (Exception e)
        {
            var message = $"Error getting user {requestingUserId}: {e.Message}";
            _logger.LogError(message);
            throw new Exception(message);
        }
    }
}