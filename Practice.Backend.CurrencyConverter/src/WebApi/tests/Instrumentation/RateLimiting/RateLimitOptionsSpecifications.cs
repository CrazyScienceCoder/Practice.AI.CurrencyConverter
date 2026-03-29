using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.RateLimiting;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.RateLimiting;

public sealed class RateLimitOptionsSpecifications
{
    [Fact]
    public void SectionName_HasCorrectValue()
    {
        RateLimitOptions.SectionName.Should().Be("RateLimit");
    }

    [Fact]
    public void PermitLimit_DefaultValue_IsOneHundred()
    {
        var options = new RateLimitOptions();

        options.PermitLimit.Should().Be(100);
    }

    [Fact]
    public void WindowSeconds_DefaultValue_IsSixty()
    {
        var options = new RateLimitOptions();

        options.WindowSeconds.Should().Be(60);
    }
}
