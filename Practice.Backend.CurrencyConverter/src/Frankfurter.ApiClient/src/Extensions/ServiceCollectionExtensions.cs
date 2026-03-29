using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Strategies;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;

public static class ServiceCollectionExtensions
{
    private const string FrankfurterApiDevV1 = "https://api.frankfurter.dev/v1/";
    private const string ApplicationJson = "application/json";

    public static IServiceCollection AddFrankfurterApiClient(this IServiceCollection services)
    {
        services.AddHttpClient<IFrankfurterApiClient, FrankfurterApiClient>(client =>
            {
                client.BaseAddress = new Uri(FrankfurterApiDevV1);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
            })
            .AddResilienceHandler(nameof(FrankfurterApiClient), (builder, context) =>
            {
                var logger = context.ServiceProvider.GetRequiredService<ILogger<FrankfurterApiClient>>();

                builder.AddRetry(HttpRetryStrategy.Create(logger))
                    .AddCircuitBreaker(HttpCircuitBreakerStrategy.Create(logger));
            });

        return services;
    }
}
