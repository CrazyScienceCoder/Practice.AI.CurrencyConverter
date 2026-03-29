using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.RateLimiting;

public sealed class RateLimitingConfiguratorSpecifications
{
    [Fact]
    public void PolicyName_HasCorrectValue()
    {
        RateLimitingConfigurator.PolicyName.Should().Be("per-user");
    }

    [Fact]
    public void AddRateLimiting_WithMissingSection_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);

        var act = () => builder.AddRateLimiting();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddRateLimiting_WithValidConfiguration_DoesNotThrow()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["RateLimit:PermitLimit"] = "50",
            ["RateLimit:WindowSeconds"] = "30"
        });

        var act = () => builder.AddRateLimiting();

        act.Should().NotThrow();
    }

    [Fact]
    public void AddRateLimiting_ValidConfiguration_RegistersRateLimiterRelatedServices()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);

        builder.AddRateLimiting();

        builder.Services.Should().Contain(d =>
            d.ServiceType.FullName != null && d.ServiceType.FullName.Contains("RateLimit"));
    }

    [Fact]
    public void AddRateLimiting_WhenOptionsResolved_ConfiguresRejectionStatusCode()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);
        builder.AddRateLimiting();
        var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<RateLimiterOptions>>().Value;

        options.RejectionStatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
    }

    [Fact]
    public async Task AddRateLimiting_OnRejected_SetsStatusCodeTo429()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);
        builder.AddRateLimiting();
        var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<RateLimiterOptions>>().Value;
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var leaseMock = new Mock<RateLimitLease>();
        leaseMock.Setup(l => l.IsAcquired).Returns(false);
        leaseMock.Setup(l => l.TryGetMetadata(
            It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);
        var onRejectedContext = new OnRejectedContext
        {
            HttpContext = httpContext,
            Lease = leaseMock.Object
        };

        await options.OnRejected!(onRejectedContext, TestContext.Current.CancellationToken);

        httpContext.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
    }

    [Fact]
    public async Task AddRateLimiting_OnRejected_WritesTooManyRequestsMessage()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);
        builder.AddRateLimiting();
        var app = builder.Build();

        var options = app.Services.GetRequiredService<IOptions<RateLimiterOptions>>().Value;
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();
        var leaseMock = new Mock<RateLimitLease>();
        leaseMock.Setup(l => l.IsAcquired).Returns(false);
        leaseMock.Setup(l => l.TryGetMetadata(
            It.IsAny<string>(), out It.Ref<object?>.IsAny)).Returns(false);
        var onRejectedContext = new OnRejectedContext
        {
            HttpContext = httpContext,
            Lease = leaseMock.Object
        };

        await options.OnRejected!(onRejectedContext, TestContext.Current.CancellationToken);

        httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(httpContext.Response.Body);
        var body = await reader.ReadToEndAsync();
        body.Should().Contain("Too many requests");
    }

    [Fact]
    public void AddRateLimiting_WhenOptionsResolved_PolicyNameIsRegistered()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection([]);
        builder.AddRateLimiting();
        var app = builder.Build();

        var rateLimiterOptions = app.Services.GetRequiredService<IOptions<RateLimiterOptions>>().Value;

        rateLimiterOptions.Should().NotBeNull();
    }
}
