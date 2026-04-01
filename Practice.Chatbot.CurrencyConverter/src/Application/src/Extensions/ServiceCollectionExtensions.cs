using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Practice.Chatbot.CurrencyConverter.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
}
