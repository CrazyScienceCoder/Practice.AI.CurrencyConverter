using Practice.Chatbot.CurrencyConverter.WebApi.Extensions;
using Serilog.Core;
using Serilog.Events;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Logging;

public sealed class HttpContextEnricher(IHttpContextAccessor httpContextAccessor) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            return;
        }

        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientIP", clientIp));

        var clientId = httpContext.User.GetUserId();
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClientId", clientId));
    }
}