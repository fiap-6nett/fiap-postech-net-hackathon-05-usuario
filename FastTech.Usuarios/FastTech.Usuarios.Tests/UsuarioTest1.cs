using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FastTech.Usuarios.Application.Services;
using FastTech.Usuarios.Application.Settings;
using FastTech.Usuarios.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;

// ou o caminho real

namespace FastTech.Usuarios.Tests;

public class UsuarioTest1
{
    [Fact]
    public void IdentitySettings_Should_Be_Bound_Correctly_From_Configuration()
    {
        // Arrange – cria um dicionário simulando o appsettings.json
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Identity:Issuer", "FastTech.Usuarios" },
            { "Identity:Audience", "FastTech.Usuarios.API" },
            { "Identity:SecretKey", "Ikja1zTw42ZG9SymDttA6Dly1kL0uZ7g2dwvdR7ZeLFnKQn5B9" },
            { "Identity:AccessTokenMinutes", "60" },
            { "Identity:AccessRefreshTokenMinutes", "43200" }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var services = new ServiceCollection();
        services.Configure<IdentitySettings>(configuration.GetSection("Identity"));

        var provider = services.BuildServiceProvider();
        var identitySettings = provider.GetRequiredService<IOptions<IdentitySettings>>().Value;

        // Assert – verifica se os valores estão corretos
        Assert.Equal("FastTech.Usuarios", identitySettings.Issuer);
        Assert.Equal("FastTech.Usuarios.API", identitySettings.Audience);
        Assert.Equal(60, identitySettings.AccessTokenMinutes);
        Assert.Equal(43200, identitySettings.AccessRefreshTokenMinutes);
    }

    [Fact]
    public async Task GenerateTokenAsync_AdminCredentials_ShouldReturnAccessToken()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserService>>();

        var identitySettings = new IdentitySettings
        {
            Issuer = "FastTech.Usuarios",
            Audience = "FastTech.Usuarios.API",
            SecretKey = "Ikja1zTw42ZG9SymDttA6Dly1kL0uZ7g2dwvdR7ZeLFnKQn5B9",
            AccessTokenMinutes = 60,
            AccessRefreshTokenMinutes = 43200
        };

        var options = Options.Create(identitySettings);
        var userService = new UserService(mockLogger.Object, options);

        var username = "admin@admin.com.br";
        var password = "admin123";
        var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        // Act
        var token = await userService.GenerateTokenAsync(username, passwordBase64, LoginIdentifierType.Cpf);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
        Assert.Contains(".", token.AccessToken); // simples validação de estrutura JWT
    }

    [Fact]
    public async Task GenerateTokenAsync_ShouldContainCorrectClaims()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserService>>();
        var identitySettings = new IdentitySettings
        {
            Issuer = "FastTech.Usuarios",
            Audience = "FastTech.Usuarios.API",
            SecretKey = "Ikja1zTw42ZG9SymDttA6Dly1kL0uZ7g2dwvdR7ZeLFnKQn5B9",
            AccessTokenMinutes = 60,
            AccessRefreshTokenMinutes = 43200
        };

        var options = Options.Create(identitySettings);
        var userService = new UserService(mockLogger.Object, options);

        var username = "admin@admin.com.br";
        var password = "admin123";
        var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        // Act
        var token = await userService.GenerateTokenAsync(username, passwordBase64, LoginIdentifierType.Email);

        // Decode JWT token
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.AccessToken);

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var userIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token.AccessToken));
        Assert.NotNull(roleClaim);
        Assert.NotNull(userIdClaim);
        Assert.Equal(UserRole.Admin.ToString(), roleClaim);
        Assert.True(Guid.TryParse(userIdClaim, out _));
    }

    [Fact]
    public async Task GenerateTokenAsync_ValidAdminToken_ShouldContainExpectedClaims()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserService>>();
        var identitySettings = new IdentitySettings
        {
            Issuer = "FastTech.Usuarios",
            Audience = "FastTech.Usuarios.API",
            SecretKey = "Ikja1zTw42ZG9SymDttA6Dly1kL0uZ7g2dwvdR7ZeLFnKQn5B9",
            AccessTokenMinutes = 60,
            AccessRefreshTokenMinutes = 43200
        };
        var options = Options.Create(identitySettings);
        var userService = new UserService(mockLogger.Object, options);

        var username = "admin@admin.com.br";
        var password = "admin123";
        var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

        // Act
        var token = await userService.GenerateTokenAsync(username, passwordBase64, LoginIdentifierType.Email);
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.AccessToken);

        // Assert
        Assert.NotNull(jwt);
        Assert.Equal("FastTech.Usuarios", jwt.Issuer);
        Assert.Equal("FastTech.Usuarios.API", jwt.Audiences.First());

        var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        Assert.Equal(UserRole.Admin.ToString(), roleClaim);
        Assert.True(Guid.TryParse(idClaim, out _));
    }

    [Fact]
    public async Task ValidateTokenAsync_WithWrongSecretKey_ShouldFailValidation()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserService>>();

        // Configuração usada para GERAR o token
        var correctSettings = new IdentitySettings
        {
            Issuer = "FastTech.Usuarios",
            Audience = "FastTech.Usuarios.API",
            SecretKey = "Ikja1zTw42ZG9SymDttA6Dly1kL0uZ7g2dwvdR7ZeLFnKQn5B9", // Chave correta
            AccessTokenMinutes = 60,
            AccessRefreshTokenMinutes = 43200
        };

        var userService = new UserService(mockLogger.Object, Options.Create(correctSettings));

        var username = "admin@admin.com.br";
        var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("admin123"));

        // Gera token com chave correta
        var token = await userService.GenerateTokenAsync(username, passwordBase64, LoginIdentifierType.Email);

        // Configuração usada para VALIDAR (com chave incorreta)
        var invalidKey = "hZLMzfmJvm2YUF0VeCMDZ3n6sSU9LS1lV5L"; // Chave incorreta

        var handler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(invalidKey)),
            ValidateIssuer = true,
            ValidIssuer = correctSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = correctSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Assert – deve lançar SecurityTokenInvalidSignatureException
        Assert.ThrowsAny<SecurityTokenInvalidSignatureException>(() => { handler.ValidateToken(token.AccessToken, validationParameters, out _); });
    }
}