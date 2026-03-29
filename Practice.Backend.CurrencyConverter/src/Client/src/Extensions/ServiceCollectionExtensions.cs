using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Practice.Backend.CurrencyConverter.Client.Auth;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Resilience;

namespace Practice.Backend.CurrencyConverter.Client.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ApplicationJson = "application/json";

    extension(IServiceCollection services)
    {
        public IServiceCollection AddCurrencyConverterClient(Action<CurrencyConverterClientOptions> configure)
        {
            services.AddOptions<CurrencyConverterClientOptions>()
                .Configure(configure)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services.RegisterHttpClient();
        }

        public IServiceCollection AddCurrencyConverterClient(IConfiguration configurationSection)
        {
            services.AddOptions<CurrencyConverterClientOptions>()
                .Bind(configurationSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services.RegisterHttpClient();
        }

        public IServiceCollection AddDefaultHttpContextTokenProvider()
        {
            services.AddScoped<ITokenProvider, DefaultHttpContextTokenProvider>();

            return services;
        }

        private IServiceCollection RegisterHttpClient()
        {
            services.AddTransient<AuthorizationDelegatingHandler>();

            services
                .AddHttpClient<ICurrencyConverterClient, CurrencyConverterClient>((serviceProvider, client) =>
                {
                    var opts = serviceProvider
                        .GetRequiredService<IOptions<CurrencyConverterClientOptions>>()
                        .Value;

                    client.BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + '/');
                    client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue(ApplicationJson));
                })
                .AddHttpMessageHandler<AuthorizationDelegatingHandler>()
                .AddResilienceHandler(
                    nameof(CurrencyConverterClient),
                    (builder, context) =>
                    {
                        var logger = context.ServiceProvider
                            .GetRequiredService<ILogger<CurrencyConverterClient>>();

                        var opts = context.ServiceProvider
                            .GetRequiredService<IOptions<CurrencyConverterClientOptions>>()
                            .Value;

                        builder
                            .AddRetry(HttpRetryStrategy.Create(logger, opts.Retry))
                            .AddCircuitBreaker(HttpCircuitBreakerStrategy.Create(logger, opts.CircuitBreaker));
                    });

            return services;
        }
    }
}
