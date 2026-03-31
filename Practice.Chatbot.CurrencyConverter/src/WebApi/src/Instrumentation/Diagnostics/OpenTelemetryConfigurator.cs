using System.Reflection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Diagnostics;

public static class OpenTelemetryConfigurator
{
    public static void AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        const string serviceName = "ai-agent-api";
        const string serviceNamespace = "Practice.Chatbot.CurrencyConverter";
        var serviceVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName: serviceName,
                    serviceVersion: serviceVersion,
                    serviceInstanceId: Environment.MachineName,
                    serviceNamespace: serviceNamespace)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName
                });
            })
            .WithTracing(tracing =>
            {
                tracing.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(probability: 0.5)))
                    .AddAspNetCoreInstrumentation(o => o.RecordException = true)
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddOtlpExporter();
            });
    }
}
