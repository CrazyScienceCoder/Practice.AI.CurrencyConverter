using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Serilog;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Bootstrap;

public static class WebApplicationExtensions
{
    public static void ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseCors();

        app.UseSwaggerEndpoint();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health");
    }
}
