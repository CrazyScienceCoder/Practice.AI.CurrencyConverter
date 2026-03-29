using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Bootstrap;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Bootstrap;

public sealed class ServiceCollectionExtensionsSpecifications
{
    [Fact]
    public void AddActionResultBuilders_RegistersActionResultBuilderFactory()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IActionResultBuilderFactory));
        descriptor.Should().NotBeNull();
        descriptor!.ImplementationType.Should().Be(typeof(ActionResultBuilderFactory));
    }

    [Fact]
    public void AddActionResultBuilders_RegistersSuccessActionResultBuilder()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IActionResultBuilder) &&
            d.ImplementationType == typeof(SuccessActionResultBuilder));
    }

    [Fact]
    public void AddActionResultBuilders_RegistersNotAllowedActionResultBuilder()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IActionResultBuilder) &&
            d.ImplementationType == typeof(NotAllowedActionResultBuilder));
    }

    [Fact]
    public void AddActionResultBuilders_RegistersNotFoundActionResultBuilder()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IActionResultBuilder) &&
            d.ImplementationType == typeof(NotFoundActionResultBuilder));
    }

    [Fact]
    public void AddActionResultBuilders_RegistersValidationErrorActionResultBuilder()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IActionResultBuilder) &&
            d.ImplementationType == typeof(ValidationErrorActionResultBuilder));
    }

    [Fact]
    public void AddActionResultBuilders_RegistersUnexpectedErrorActionResultBuilder()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        services.Should().Contain(d =>
            d.ServiceType == typeof(IActionResultBuilder) &&
            d.ImplementationType == typeof(UnexpectedErrorActionResultBuilder));
    }

    [Fact]
    public void AddActionResultBuilders_AllBuildersRegisteredAsScoped()
    {
        var services = new ServiceCollection();
        services.AddHttpContextAccessor();
        services.AddMvcCore();

        services.AddActionResultBuilders();

        var builderDescriptors = services
            .Where(d => d.ServiceType == typeof(IActionResultBuilder))
            .ToList();

        builderDescriptors.Should().AllSatisfy(d =>
            d.Lifetime.Should().Be(ServiceLifetime.Scoped));
    }

    [Fact]
    public void AddApiVersioning_DoesNotThrow()
    {
        var services = new ServiceCollection();

        // Qualify explicitly to avoid ambiguity with Asp.Versioning's own AddApiVersioning overload
        var act = () => WebApiVersioningExtensionsCaller.Call(services);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddApiVersioning_RegistersVersioningServices()
    {
        var services = new ServiceCollection();

        WebApiVersioningExtensionsCaller.Call(services);

        services.Should().NotBeEmpty();
    }

    // Helper to resolve ambiguity: calls WebApi's AddApiVersioning that configures
    // DefaultApiVersion, AssumeDefaultVersionWhenUnspecified, ReportApiVersions and UrlSegmentApiVersionReader.
    private static class WebApiVersioningExtensionsCaller
    {
        public static void Call(IServiceCollection services)
            => WebApi.Bootstrap.ServiceCollectionExtensions.AddApiVersioning(services);
    }
}
