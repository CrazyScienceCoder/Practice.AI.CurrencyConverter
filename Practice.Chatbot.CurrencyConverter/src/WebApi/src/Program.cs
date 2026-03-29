using Practice.Chatbot.CurrencyConverter.WebApi.Bootstrap;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Diagnostics;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Logging;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = SerilogConfigurator.CreateBootstrapLogger(builder.Configuration);

try
{
    Log.Information("AI Agent API is starting");

    builder.UseSerilog();

    builder.AddCoreServices();

    builder.AddSwagger();

    builder.AddOpenTelemetry();

    var app = builder.Build();

    app.ConfigurePipeline();

    app.Run();

    Log.Information("AI Agent API stopped");
}
catch (Exception ex)
{
    Log.Error(ex, "Stopped AI Agent API because of unhandled exception");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
