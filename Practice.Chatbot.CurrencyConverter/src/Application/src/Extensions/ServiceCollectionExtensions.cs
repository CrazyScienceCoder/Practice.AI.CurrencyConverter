using Microsoft.Extensions.DependencyInjection;

namespace Practice.Chatbot.CurrencyConverter.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<IAssemblyMarker>());

        return services;
    }
}
