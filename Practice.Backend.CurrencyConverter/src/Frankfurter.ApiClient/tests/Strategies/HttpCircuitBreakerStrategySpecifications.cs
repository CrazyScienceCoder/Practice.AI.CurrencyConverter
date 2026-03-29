using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Strategies;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Strategies;

public sealed class HttpCircuitBreakerStrategySpecifications
{
    [Fact]
    public void Create_WithLogger_ReturnsHttpCircuitBreakerStrategyOptions()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.Should().NotBeNull();
        result.Should().BeOfType<HttpCircuitBreakerStrategyOptions>();
    }

    [Fact]
    public void Create_WithLogger_SetsFailureRatioToFiftyPercent()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.FailureRatio.Should().Be(0.5);
    }

    [Fact]
    public void Create_WithLogger_SetsBreakDurationToSixtySeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.BreakDuration.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void Create_WithLogger_SetsSamplingDurationToSixtySeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.SamplingDuration.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void Create_WithLogger_SetsMinimumThroughputToThirty()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.MinimumThroughput.Should().Be(30);
    }

    [Fact]
    public void Create_WithLogger_OnOpenedDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.OnOpened.Should().NotBeNull();
    }

    [Fact]
    public void Create_WithLogger_OnClosedDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpCircuitBreakerStrategy.Create(loggerMock.Object);

        result.OnClosed.Should().NotBeNull();
    }

    [Fact]
    public async Task OnOpened_WhenInvoked_LogsError()
    {
        var loggerMock = new Mock<ILogger>();
        var options = HttpCircuitBreakerStrategy.Create(loggerMock.Object);
        var context = ResilienceContextPool.Shared.Get(TestContext.Current.CancellationToken);
        var outcome = Outcome.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        var args = new OnCircuitOpenedArguments<HttpResponseMessage>(context, outcome, TimeSpan.FromSeconds(60), false);

        await options.OnOpened!(args);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Error
                , It.IsAny<EventId>()
                , It.IsAny<It.IsAnyType>()
                , It.IsAny<Exception?>()
                , It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
    }

    [Fact]
    public async Task OnClosed_WhenInvoked_LogsInformation()
    {
        var loggerMock = new Mock<ILogger>();
        var options = HttpCircuitBreakerStrategy.Create(loggerMock.Object);
        var context = ResilienceContextPool.Shared.Get(TestContext.Current.CancellationToken);
        var outcome = Outcome.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.OK));
        var args = new OnCircuitClosedArguments<HttpResponseMessage>(context, outcome, false);

        await options.OnClosed!(args);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Information
                , It.IsAny<EventId>()
                , It.IsAny<It.IsAnyType>()
                , It.IsAny<Exception?>()
                , It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
    }
}
