using FluentValidation;
using Practice.Chatbot.CurrencyConverter.Application.Extensions;
using Practice.Chatbot.CurrencyConverter.Infrastructure.Extensions;
using Practice.Chatbot.CurrencyConverter.WebApi.Filters;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Authentication;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Logging;
using Serilog.Core;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Bootstrap;

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

        builder.Services.AddApplication();

        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddActionResultBuilders();

        builder.Services.AddTransient<ILogEventEnricher, HttpContextEnricher>();

        builder.AddKeycloakJwtAuth();

        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });
    }
}
