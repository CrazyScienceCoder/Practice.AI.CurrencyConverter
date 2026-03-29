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
            options.Filters.Add<FluentValidationActionFilter>();
        });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddTransient<ILogEventEnricher, HttpContextEnricher>();

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();

        builder.Services.AddApplication();

        builder.Services.AddInfrastructure(builder.Configuration);

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
