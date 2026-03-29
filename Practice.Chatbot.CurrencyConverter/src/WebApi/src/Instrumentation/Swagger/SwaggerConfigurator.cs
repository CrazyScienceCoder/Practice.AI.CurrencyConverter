using Asp.Versioning.ApiExplorer;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Swagger;

public static class SwaggerConfigurator
{
    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
    }

    public static void UseSwaggerEndpoint(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());
            }
        });
    }
}
