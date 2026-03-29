using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Practice.Chatbot.CurrencyConverter.Application.Abstractions;

namespace Practice.Chatbot.CurrencyConverter.Integration.Tests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    internal const string TestSigningKey =
        "integration-test-signing-key-hmac-sha256-min-32-bytes!";

    public Mock<IChatOrchestrator> ChatOrchestratorMock { get; } = new(MockBehavior.Loose);

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
            ReplaceChatOrchestrator(services);
            OverrideJwtAuthentication(services);
        });
    }

    private static void ReplaceDistributedCache(IServiceCollection services)
    {
        services.RemoveAll<IDistributedCache>();
        services.AddDistributedMemoryCache();
    }

    private void ReplaceChatOrchestrator(IServiceCollection services)
    {
        services.RemoveAll<IChatOrchestrator>();
        services.AddScoped(_ => ChatOrchestratorMock.Object);
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
}
