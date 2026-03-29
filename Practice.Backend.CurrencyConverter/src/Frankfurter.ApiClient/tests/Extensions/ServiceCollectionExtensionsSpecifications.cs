using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Extensions;

public sealed class ServiceCollectionExtensionsSpecifications
{
    [Fact]
    public void AddFrankfurterApiClient_WithValidServices_ReturnsSameServicesInstance()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var result = services.AddFrankfurterApiClient();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddFrankfurterApiClient_WithValidServices_RegistersIFrankfurterApiClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddFrankfurterApiClient();

        services.Any(sd => sd.ServiceType == typeof(IFrankfurterApiClient)).Should().BeTrue();
    }

    [Fact]
    public void AddFrankfurterApiClient_WithValidServices_CanResolveIFrankfurterApiClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddFrankfurterApiClient();

        var provider = services.BuildServiceProvider();
        var client = provider.GetService<IFrankfurterApiClient>();

        client.Should().NotBeNull();
    }

    [Fact]
    public void AddFrankfurterApiClient_CalledTwice_DoesNotThrow()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        var act = () =>
        {
            services.AddFrankfurterApiClient();
            services.AddFrankfurterApiClient();
        };

        act.Should().NotThrow();
    }
}
