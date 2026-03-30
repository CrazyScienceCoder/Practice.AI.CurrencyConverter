using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Client.Auth;
using Practice.Backend.CurrencyConverter.Client.Extensions;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Extensions;

public sealed class ServiceCollectionExtensionsSpecifications
{
    [Fact]
    public void AddCurrencyConverterClient_WithConfigureAction_ReturnsSameServicesInstance()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var result = services.AddCurrencyConverterClient(
            opts => opts.BaseUrl = "http://localhost:5263");

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigureAction_RegistersICurrencyConverterClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddCurrencyConverterClient(opts => opts.BaseUrl = "http://localhost:5263");

        services.Any(sd => sd.ServiceType == typeof(ICurrencyConverterClient))
            .Should().BeTrue();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigureAction_RegistersAuthorizationDelegatingHandlerAsTransient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddCurrencyConverterClient(opts => opts.BaseUrl = "http://localhost:5263");

        services.Any(sd =>
                sd.ServiceType == typeof(AuthorizationDelegatingHandler) &&
                sd.Lifetime == ServiceLifetime.Transient)
            .Should().BeTrue();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigureAction_CanResolveICurrencyConverterClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<ITokenProvider>());

        services.AddCurrencyConverterClient(opts => opts.BaseUrl = "http://localhost:5263");

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<ICurrencyConverterClient>();

        client.Should().NotBeNull();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigureAction_ResolvedClientImplementsInterface()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<ITokenProvider>());

        services.AddCurrencyConverterClient(opts => opts.BaseUrl = "http://localhost:5263");

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<ICurrencyConverterClient>();

        client.Should().BeAssignableTo<ICurrencyConverterClient>();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigurationSection_ReturnsSameServicesInstance()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfiguration("http://localhost:5263");

        var result = services.AddCurrencyConverterClient(config);

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigurationSection_RegistersICurrencyConverterClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfiguration("http://localhost:5263");

        services.AddCurrencyConverterClient(config);

        services.Any(sd => sd.ServiceType == typeof(ICurrencyConverterClient))
            .Should().BeTrue();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigurationSection_RegistersAuthorizationDelegatingHandlerAsTransient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        var config = BuildConfiguration("http://localhost:5263");

        services.AddCurrencyConverterClient(config);

        services.Any(sd =>
                sd.ServiceType == typeof(AuthorizationDelegatingHandler) &&
                sd.Lifetime == ServiceLifetime.Transient)
            .Should().BeTrue();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigurationSection_CanResolveICurrencyConverterClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<ITokenProvider>());
        var config = BuildConfiguration("http://localhost:5263");

        services.AddCurrencyConverterClient(config);

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<ICurrencyConverterClient>();

        client.Should().NotBeNull();
    }

    [Fact]
    public void AddCurrencyConverterClient_WithConfigurationSection_ResolvedClientImplementsInterface()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(Mock.Of<ITokenProvider>());
        var config = BuildConfiguration("http://localhost:5263");

        services.AddCurrencyConverterClient(config);

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<ICurrencyConverterClient>();

        client.Should().BeAssignableTo<ICurrencyConverterClient>();
    }

    [Fact]
    public void AddDefaultHttpContextTokenProvider_RegistersITokenProvider()
    {
        var services = new ServiceCollection();

        services.AddDefaultHttpContextTokenProvider();

        services.Any(sd => sd.ServiceType == typeof(ITokenProvider))
            .Should().BeTrue();
    }

    [Fact]
    public void AddDefaultHttpContextTokenProvider_RegistersAsScoped()
    {
        var services = new ServiceCollection();

        services.AddDefaultHttpContextTokenProvider();

        services.Any(sd =>
                sd.ServiceType == typeof(ITokenProvider) &&
                sd.Lifetime == ServiceLifetime.Scoped)
            .Should().BeTrue();
    }

    [Fact]
    public void AddDefaultHttpContextTokenProvider_RegistersDefaultHttpContextTokenProviderAsImplementation()
    {
        var services = new ServiceCollection();

        services.AddDefaultHttpContextTokenProvider();

        services.Any(sd =>
                sd.ServiceType == typeof(ITokenProvider) &&
                sd.ImplementationType == typeof(DefaultHttpContextTokenProvider))
            .Should().BeTrue();
    }

    private static IConfiguration BuildConfiguration(string baseUrl)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["BaseUrl"] = baseUrl
            })
            .Build();
}
