using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Resilience;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Resilience;

public sealed class HttpCircuitBreakerStrategySpecifications
{
    private static CurrencyConverterClientOptions.CircuitBreakerOptions DefaultOptions => new();

    [Fact]
    public void Create_WithOptions_ReturnsHttpCircuitBreakerStrategyOptions()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.Should().NotBeNull();
        result.Should().BeOfType<HttpCircuitBreakerStrategyOptions>();
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsFailureRatioToFiftyPercent()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.FailureRatio.Should().Be(0.5);
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsBreakDurationToSixtySeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.BreakDuration.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsSamplingDurationToSixtySeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.SamplingDuration.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsMinimumThroughputToThirty()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.MinimumThroughput.Should().Be(30);
    }

    [Fact]
    public void Create_WithDefaultOptions_OnOpenedDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.OnOpened.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithDefaultOptions_OnClosedDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, DefaultOptions);

        result.OnClosed.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(0.5)]
    [InlineData(1.0)]
    public void Create_WithCustomFailureRatio_SetsCorrectFailureRatio(double failureRatio)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions { FailureRatio = failureRatio };

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, options);

        result.FailureRatio.Should().Be(failureRatio);
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void Create_WithCustomBreakDuration_SetsCorrectBreakDuration(int breakDurationSeconds)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions { BreakDurationSeconds = breakDurationSeconds };

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, options);

        result.BreakDuration.Should().Be(TimeSpan.FromSeconds(breakDurationSeconds));
    }

    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    [InlineData(120)]
    public void Create_WithCustomSamplingDuration_SetsCorrectSamplingDuration(int samplingDurationSeconds)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions { SamplingDurationSeconds = samplingDurationSeconds };

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, options);

        result.SamplingDuration.Should().Be(TimeSpan.FromSeconds(samplingDurationSeconds));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(30)]
    [InlineData(100)]
    public void Create_WithCustomMinimumThroughput_SetsCorrectMinimumThroughput(int minimumThroughput)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.CircuitBreakerOptions { MinimumThroughput = minimumThroughput };

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object, options);

        result.MinimumThroughput.Should().Be(minimumThroughput);
    }
}
