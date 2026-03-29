using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Authentication;

public sealed class JwtAuthOptionsSpecifications
{
    [Fact]
    public void SectionName_HasCorrectValue()
    {
        JwtAuthOptions.SectionName.Should().Be("JwtAuth");
    }

    [Fact]
    public void RequireHttpsMetadata_DefaultValue_IsTrue()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.RequireHttpsMetadata.Should().BeTrue();
    }

    [Fact]
    public void ValidateIssuer_DefaultValue_IsTrue()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ValidateIssuer.Should().BeTrue();
    }

    [Fact]
    public void ValidateAudience_DefaultValue_IsTrue()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ValidateAudience.Should().BeTrue();
    }

    [Fact]
    public void ValidateLifetime_DefaultValue_IsTrue()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ValidateLifetime.Should().BeTrue();
    }

    [Fact]
    public void ValidateIssuerSigningKey_DefaultValue_IsTrue()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ValidateIssuerSigningKey.Should().BeTrue();
    }

    [Fact]
    public void ClockSkewSeconds_DefaultValue_IsZero()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ClockSkewSeconds.Should().Be(0);
    }

    [Fact]
    public void ValidIssuer_DefaultValue_IsNull()
    {
        var options = new JwtAuthOptions { Authority = "http://localhost", Audience = "api" };

        options.ValidIssuer.Should().BeNull();
    }
}
