using FastTech.Usuarios.Application.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            { "Identity:SecretKey", "toucan-01.lmq.cloudamqp.com" },
            { "Identity:AccessTokenMinutes", "60" },
            { "Identity:RefreshTokenMinutes", "43200" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var services = new ServiceCollection();
        services.Configure<IdentitySettings>(configuration.GetSection("Identity"));

        var provider = services.BuildServiceProvider();
        var identitySettings = provider.GetRequiredService<IOptions<IdentitySettings>>().Value;

        // Assert – verifica se os valores estão corretos
        Assert.Equal("FastTech.Usuarios", identitySettings.Issuer);
        Assert.Equal("FastTech.Usuarios.API", identitySettings.Audience);
        Assert.Equal("toucan-01.lmq.cloudamqp.com", identitySettings.SecretKey);
        Assert.Equal(60, identitySettings.AccessTokenMinutes);
        Assert.Equal(43200, identitySettings.AccessRefreshTokenMinutes);
    }
}