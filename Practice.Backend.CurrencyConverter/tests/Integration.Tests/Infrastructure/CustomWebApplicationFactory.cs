using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    internal const string TestSigningKey =
        "integration-test-signing-key-hmac-sha256-min-32-bytes!";

    public Mock<IFrankfurterApiClient> FrankfurterClientMock { get; } = new(MockBehavior.Loose);

    public CustomWebApplicationFactory()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        if (Environment.GetEnvironmentVariable("OPENSEARCH_NODE_URIS") is null)
        {
            Environment.SetEnvironmentVariable("OPENSEARCH_NODE_URIS", "http://localhost:9200");
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(
                Path.Combine(AppContext.BaseDirectory, "appsettings.Testing.json"),
                optional: false,
                reloadOnChange: false);
        });

        builder.ConfigureServices(services =>
        {
            ReplaceDistributedCache(services);
            ReplaceFrankfurterApiClient(services);
            ReplaceConnectionMultiplexer(services);
            OverrideJwtAuthentication(services);
        });
    }

    private static void ReplaceDistributedCache(IServiceCollection services)
    {
        services.RemoveAll<IDistributedCache>();
        services.AddDistributedMemoryCache();
    }

    private void ReplaceFrankfurterApiClient(IServiceCollection services)
    {
        services.RemoveAll<IFrankfurterApiClient>();
        services.AddSingleton(FrankfurterClientMock.Object);
    }

    private static void ReplaceConnectionMultiplexer(IServiceCollection services)
    {
        services.RemoveAll<IConnectionMultiplexer>();
        services.AddSingleton(CreateMockConnectionMultiplexer());
    }

    private static void OverrideJwtAuthentication(IServiceCollection services)
    {
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = null;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(TestSigningKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };
        });
    }

    private static IConnectionMultiplexer CreateMockConnectionMultiplexer()
    {
        var db = new Mock<IDatabase>(MockBehavior.Loose);

        var permitResult = RedisResult.Create([
            RedisResult.Create(true), // index 0: allowed = true
            RedisResult.Create(1L) // index 1: current count
        ]);

        db.Setup(d => d.ScriptEvaluateAsync(
                It.IsAny<string>(),
                It.IsAny<RedisKey[]?>(),
                It.IsAny<RedisValue[]?>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(permitResult);

        db.Setup(d => d.ScriptEvaluateAsync(
                It.IsAny<LuaScript>(),
                It.IsAny<object?>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(permitResult);

        db.Setup(d => d.ScriptEvaluateAsync(
                It.IsAny<LoadedLuaScript>(),
                It.IsAny<object?>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(permitResult);

        var multiplexer = new Mock<IConnectionMultiplexer>(MockBehavior.Loose);
        multiplexer
            .Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(db.Object);
        multiplexer
            .Setup(m => m.IsConnected)
            .Returns(true);

        return multiplexer.Object;
    }
}
