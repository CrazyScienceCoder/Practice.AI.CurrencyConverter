using Asp.Versioning;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

namespace Practice.Backend.CurrencyConverter.WebApi.Bootstrap;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddActionResultBuilders()
        {
            services.AddScoped<IActionResultBuilderFactory, ActionResultBuilderFactory>();

            services.AddScoped<IActionResultBuilder, SuccessActionResultBuilder>();
            services.AddScoped<IActionResultBuilder, NotAllowedActionResultBuilder>();
            services.AddScoped<IActionResultBuilder, NotFoundActionResultBuilder>();
            services.AddScoped<IActionResultBuilder, ValidationErrorActionResultBuilder>();
            services.AddScoped<IActionResultBuilder, UnexpectedErrorActionResultBuilder>();
        }

        public void AddApiVersioning()
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        }
    }
}
