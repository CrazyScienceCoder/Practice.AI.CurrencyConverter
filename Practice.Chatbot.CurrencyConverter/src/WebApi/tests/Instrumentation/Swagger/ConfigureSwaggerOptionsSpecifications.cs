using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Tests.Instrumentation.Swagger;

public class ConfigureSwaggerOptionsSpecifications
{
    [Fact]
    public void Configure_AddsSwaggerDocForEachApiVersion()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKey("v1");
    }

    [Fact]
    public void Configure_WithMultipleVersions_AddsDocForEach()
    {
        var descriptions = new[]
        {
            new ApiVersionDescription(new ApiVersion(1, 0), "v1"),
            new ApiVersionDescription(new ApiVersion(2, 0), "v2")
        };
        var (configurator, options) = BuildWith(descriptions);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SwaggerDocs.Should().ContainKeys("v1", "v2");
    }

    [Fact]
    public void Configure_SwaggerDocTitleContainsApiVersionNumber()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Title.Should().Contain("1.0");
    }

    [Fact]
    public void Configure_SwaggerDocTitleContainsAiAgentApi()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SwaggerDocs["v1"].Title.Should().Contain("AI Agent API");
    }

    [Fact]
    public void Configure_AddsBearerSecurityDefinition()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SecuritySchemes.Should().ContainKey("Bearer");
    }

    [Fact]
    public void Configure_BearerSchemeIsHttp()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"].Scheme.Should().Be("bearer");
    }

    [Fact]
    public void Configure_AddsSecurityRequirement()
    {
        var (configurator, options) = BuildWith([new ApiVersionDescription(new ApiVersion(1, 0), "v1")]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SecurityRequirements.Should().NotBeEmpty();
    }

    [Fact]
    public void Configure_WithNoVersions_AddsBearerSecurityDefinitionOnly()
    {
        var (configurator, options) = BuildWith([]);

        configurator.Configure(options);

        options.SwaggerGeneratorOptions.SwaggerDocs.Should().BeEmpty();
        options.SwaggerGeneratorOptions.SecuritySchemes.Should().ContainKey("Bearer");
    }

    private static (ConfigureSwaggerOptions configurator, SwaggerGenOptions options) BuildWith(
        IEnumerable<ApiVersionDescription> descriptions)
    {
        var providerMock = new Mock<IApiVersionDescriptionProvider>();
        providerMock
            .Setup(x => x.ApiVersionDescriptions)
            .Returns(descriptions.ToList().AsReadOnly());

        return (new ConfigureSwaggerOptions(providerMock.Object), new SwaggerGenOptions());
    }
}
