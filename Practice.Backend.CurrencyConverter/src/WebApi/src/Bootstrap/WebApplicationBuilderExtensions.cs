using FluentValidation;
using Practice.Backend.CurrencyConverter.Application.Extensions;
using Practice.Backend.CurrencyConverter.Infrastructure.Extensions;
using Practice.Backend.CurrencyConverter.WebApi.Filters;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.HealthChecks;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;
using Serilog.Core;

namespace Practice.Backend.CurrencyConverter.WebApi.Bootstrap;

public static class WebApplicationBuilderExtensions
{
    public static void AddCoreServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<ExceptionFilter>();
            options.Filters.Add<FluentValidationActionFilter>();
        });

        builder.Services.AddApiVersioning();

        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddApplication(builder.Configuration);

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddActionResultBuilders();

        builder.Services.AddTransient<ILogEventEnricher, HttpContextEnricher>();

        builder.AddJwtAuth();

        builder.AddRateLimiting();

        builder.Services.AddHealthChecks()
            .AddCheck<RedisHealthCheck>("redis");
    }
}