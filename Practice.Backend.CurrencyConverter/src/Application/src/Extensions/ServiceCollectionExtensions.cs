using System.Reflection;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.Behaviors;
using Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

namespace Practice.Backend.CurrencyConverter.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(
            c => c.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CurrencyPolicyBehavior<,>));

        services.AddSingleton<ICurrencyPolicy, CurrencyPolicy>();
    }
}
