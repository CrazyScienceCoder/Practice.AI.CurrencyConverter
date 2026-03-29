using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Authentication;

public sealed class AuthenticationConfiguratorSpecifications
{
    [Fact]
    public void AddJwtAuth_MissingJwtAuthSection_ThrowsInvalidOperationException()
    {
        var builder = CreateIsolatedBuilder([]);

        var act = () => builder.AddJwtAuth();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*JwtAuth*");
    }

    [Fact]
    public void AddJwtAuth_ValidConfiguration_RegistersAuthenticationService()
    {
        var builder = CreateIsolatedBuilder(ValidConfig());

        builder.AddJwtAuth();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IAuthenticationService));
    }

    [Fact]
    public void AddJwtAuth_ValidConfiguration_RegistersAuthorizationService()
    {
        var builder = CreateIsolatedBuilder(ValidConfig());

        builder.AddJwtAuth();

        builder.Services.Should().Contain(d => d.ServiceType == typeof(IAuthorizationService));
    }

    [Fact]
    public void AddJwtAuth_ValidConfiguration_ConfiguresJwtBearerAuthority()
    {
        var builder = CreateIsolatedBuilder(ValidConfig());
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.Authority.Should().Be("http://localhost:8080/realms/test");
    }

    [Fact]
    public void AddJwtAuth_ValidConfiguration_ConfiguresJwtBearerAudience()
    {
        var builder = CreateIsolatedBuilder(ValidConfig());
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.Audience.Should().Be("currency-api");
    }

    [Fact]
    public void AddJwtAuth_WithoutValidIssuer_UsesAuthorityAsValidIssuer()
    {
        var builder = CreateIsolatedBuilder(ValidConfig());
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ValidIssuer.Should().Be("http://localhost:8080/realms/test");
    }

    [Fact]
    public void AddJwtAuth_WithValidIssuer_UsesExplicitValidIssuer()
    {
        const string explicitIssuer = "http://public.example.com/realms/test";
        var config = ValidConfig();
        config["JwtAuth:ValidIssuer"] = explicitIssuer;

        var builder = CreateIsolatedBuilder(config);
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ValidIssuer.Should().Be(explicitIssuer);
    }

    [Fact]
    public void AddJwtAuth_WithoutRequireHttpsMetadata_DefaultsToTrue()
    {
        var config = new Dictionary<string, string?>
        {
            ["JwtAuth:Authority"] = "https://localhost:8080/realms/test",
            ["JwtAuth:Audience"] = "currency-api"
        };
        var builder = CreateIsolatedBuilder(config);
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.RequireHttpsMetadata.Should().BeTrue();
    }

    [Fact]
    public void AddJwtAuth_ValidConfiguration_SetsClockSkew()
    {
        var config = ValidConfig();
        config["JwtAuth:ClockSkewSeconds"] = "30";
        var builder = CreateIsolatedBuilder(config);
        builder.AddJwtAuth();
        var app = builder.Build();

        var options = app.Services
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.FromSeconds(30));
    }

    [Fact]
    public void Policies_CurrencyRead_HasExpectedValue()
    {
        Policies.CurrencyRead.Should().Be("currency:read");
    }

    [Fact]
    public void Policies_CurrencyAdmin_HasExpectedValue()
    {
        Policies.CurrencyAdmin.Should().Be("currency:admin");
    }

    private static WebApplicationBuilder CreateIsolatedBuilder(Dictionary<string, string?> config)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(config);
        return builder;
    }

    private static Dictionary<string, string?> ValidConfig() => new()
    {
        ["JwtAuth:Authority"] = "http://localhost:8080/realms/test",
        ["JwtAuth:Audience"] = "currency-api",
        ["JwtAuth:RequireHttpsMetadata"] = "false"
    };
}
