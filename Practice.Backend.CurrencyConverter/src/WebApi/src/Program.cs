using Practice.Backend.CurrencyConverter.WebApi.Bootstrap;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Diagnostics;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfigurator.CreateBootstrapLogger(builder.Configuration);
try
{
    Log.Information("Application is starting");
    //trigger the build
    builder.UseSerilog();

    builder.AddCoreServices();

    builder.AddSwagger();

    builder.AddOpenTelemetry();

    var app = builder.Build();

    app.ConfigurePipeline();

    app.Run();

    Log.Information("Application stopped");
}
catch (Exception ex)
{
    Log.Error(ex, "Stopped program because of unhandled exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
