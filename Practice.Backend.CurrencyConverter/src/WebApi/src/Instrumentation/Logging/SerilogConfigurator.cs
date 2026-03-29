using System.Diagnostics;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;

public static class SerilogConfigurator
{
    public static ILogger CreateBootstrapLogger(IConfiguration configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.With<ActivityEnricher>()
            .CreateLogger();
    }

    public static void UseSerilog(this WebApplicationBuilder builder)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        builder.Host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.With<ActivityEnricher>();
        });
    }
}