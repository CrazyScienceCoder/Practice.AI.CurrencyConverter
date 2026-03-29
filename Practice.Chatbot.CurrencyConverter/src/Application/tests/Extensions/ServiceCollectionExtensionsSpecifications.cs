using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Practice.Chatbot.CurrencyConverter.Application.Extensions;

namespace Practice.Chatbot.CurrencyConverter.Application.Tests.Extensions;

public class ServiceCollectionExtensionsSpecifications
{
    [Fact]
    public void AddApplication_RegistersMediatorInServiceCollection()
    {
        var services = new ServiceCollection();

        services.AddApplication();

        services.Should().Contain(d => d.ServiceType == typeof(IMediator));
    }

    [Fact]
    public void AddApplication_ReturnsSameServiceCollection()
    {
        var services = new ServiceCollection();

        var result = services.AddApplication();

        result.Should().BeSameAs(services);
    }
}
