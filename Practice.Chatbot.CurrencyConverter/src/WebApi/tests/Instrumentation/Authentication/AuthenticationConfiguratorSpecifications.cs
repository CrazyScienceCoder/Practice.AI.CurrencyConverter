using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Instrumentation.Authentication;

public class AuthenticationConfiguratorSpecifications
{
    [Fact]
    public void AddKeycloakJwtAuth_MissingAuthority_ThrowsInvalidOperationException()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false"
            // Keycloak:Authority deliberately omitted
        });

        var act = () => builder.AddKeycloakJwtAuth();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Authority*");
    }

    [Fact]
    public void AddKeycloakJwtAuth_MissingAudience_ThrowsInvalidOperationException()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:RequireHttpsMetadata"] = "false"
            // Keycloak:Audience deliberately omitted
        });

        var act = () => builder.AddKeycloakJwtAuth();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Audience*");
    }

    [Fact]
    public void AddKeycloakJwtAuth_ValidConfiguration_RegistersAuthenticationService()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false"
        });

        builder.AddKeycloakJwtAuth();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IAuthenticationService));
    }

    [Fact]
    public void AddKeycloakJwtAuth_ValidConfiguration_RegistersAuthorizationService()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false"
        });

        builder.AddKeycloakJwtAuth();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IAuthorizationService));
    }

    [Fact]
    public void AddKeycloakJwtAuth_ValidConfiguration_ConfiguresJwtBearerAuthority()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false"
        });
        builder.AddKeycloakJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.Authority.Should().Be("http://localhost:8080/realms/test");
        options.Audience.Should().Be("test-api");
        options.RequireHttpsMetadata.Should().BeFalse();
    }

    [Fact]
    public void AddKeycloakJwtAuth_WithoutValidIssuer_UsesAuthorityAsValidIssuer()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false"
            // Keycloak:ValidIssuer omitted — should fall back to Authority
        });
        builder.AddKeycloakJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ValidIssuer
            .Should().Be("http://localhost:8080/realms/test");
    }

    [Fact]
    public void AddKeycloakJwtAuth_WithValidIssuer_UsesExplicitValidIssuer()
    {
        const string explicitIssuer = "http://public.example.com/realms/test";
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "http://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api",
            ["Keycloak:RequireHttpsMetadata"] = "false",
            ["Keycloak:ValidIssuer"] = explicitIssuer
        });
        builder.AddKeycloakJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ValidIssuer.Should().Be(explicitIssuer);
    }

    [Fact]
    public void AddKeycloakJwtAuth_WithoutRequireHttpsMetadata_DefaultsToTrue()
    {
        var builder = CreateIsolatedBuilder(new Dictionary<string, string?>
        {
            ["Keycloak:Authority"] = "https://localhost:8080/realms/test",
            ["Keycloak:Audience"] = "test-api"
            // Keycloak:RequireHttpsMetadata omitted — should default to true
        });
        builder.AddKeycloakJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.RequireHttpsMetadata.Should().BeTrue();
    }

    [Fact]
    public void Policies_AiChat_HasExpectedValue()
    {
        Policies.AiChat.Should().Be("ai:chat");
    }

    // Creates a builder with cleared configuration sources so that appsettings.json
    // values (copied to test output) do not interfere with the test's expected config.
    private static WebApplicationBuilder CreateIsolatedBuilder(Dictionary<string, string?> config)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(config);
        return builder;
    }
}
