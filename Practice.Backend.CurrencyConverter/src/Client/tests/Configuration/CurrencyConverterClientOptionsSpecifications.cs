using Practice.Backend.CurrencyConverter.Client.Configuration;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Configuration;

public sealed class CurrencyConverterClientOptionsSpecifications
{
    [Fact]
    public void SectionName_HasCorrectValue()
    {
        CurrencyConverterClientOptions.SectionName.Should().Be("CurrencyConverterClient");
    }

    [Fact]
    public void ApiVersion_DefaultValue_IsOne()
    {
        var options = new CurrencyConverterClientOptions();

        options.ApiVersion.Should().Be("1");
    }

    [Fact]
    public void TimeoutSeconds_DefaultValue_IsThirty()
    {
        var options = new CurrencyConverterClientOptions();

        options.TimeoutSeconds.Should().Be(30);
    }

    [Fact]
    public void Retry_DefaultValue_IsNotNull()
    {
        var options = new CurrencyConverterClientOptions();

        options.Retry.Should().NotBeNull();
    }

    [Fact]
    public void CircuitBreaker_DefaultValue_IsNotNull()
    {
        var options = new CurrencyConverterClientOptions();

        options.CircuitBreaker.Should().NotBeNull();
    }

    [Fact]
    public void RetryOptions_MaxAttempts_DefaultValue_IsThree()
    {
        var options = new CurrencyConverterClientOptions.RetryOptions();

        options.MaxAttempts.Should().Be(3);
    }

    [Fact]
    public void RetryOptions_InitialDelaySeconds_DefaultValue_IsTwo()
    {
        var options = new CurrencyConverterClientOptions.RetryOptions();

        options.InitialDelaySeconds.Should().Be(2);
    }

    [Fact]
    public void CircuitBreakerOptions_FailureRatio_DefaultValue_IsPointFive()
    {
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions();

        options.FailureRatio.Should().Be(0.5);
    }

    [Fact]
    public void CircuitBreakerOptions_BreakDurationSeconds_DefaultValue_IsSixty()
    {
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions();

        options.BreakDurationSeconds.Should().Be(60);
    }

    [Fact]
    public void CircuitBreakerOptions_SamplingDurationSeconds_DefaultValue_IsSixty()
    {
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions();

        options.SamplingDurationSeconds.Should().Be(60);
    }

    [Fact]
    public void CircuitBreakerOptions_MinimumThroughput_DefaultValue_IsThirty()
    {
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions();

        options.MinimumThroughput.Should().Be(30);
    }
}
