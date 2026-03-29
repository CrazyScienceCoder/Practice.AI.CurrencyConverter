using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Behaviors;
using Practice.Backend.CurrencyConverter.Application.Extensions;
using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

namespace Practice.Backend.CurrencyConverter.Application.Tests.Extensions;

public sealed class ServiceCollectionExtensionsSpecifications
{
    [Fact]
    public void AddApplication_RegistersICurrencyPolicySingleton()
    {
        var services = new ServiceCollection();

        services.AddApplication(new Mock<IConfiguration>().Object);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrencyPolicy));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Singleton);
        descriptor.ImplementationType.Should().Be(typeof(CurrencyPolicy));
    }

    [Fact]
    public void AddApplication_RegistersCurrencyPolicyBehaviorAsTransient()
    {
        var services = new ServiceCollection();

        services.AddApplication(new Mock<IConfiguration>().Object);

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IPipelineBehavior<,>));
        descriptor.Should().NotBeNull();
        descriptor!.Lifetime.Should().Be(ServiceLifetime.Transient);
        descriptor.ImplementationType.Should().Be(typeof(CurrencyPolicyBehavior<,>));
    }

    [Fact]
    public void AddApplication_RegistersMediatRServices()
    {
        var services = new ServiceCollection();

        services.AddApplication(new Mock<IConfiguration>().Object);

        services.Should().Contain(d => d.ServiceType == typeof(IMediator));
    }
}
