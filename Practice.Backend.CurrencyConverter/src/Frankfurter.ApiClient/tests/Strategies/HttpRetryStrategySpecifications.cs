using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Strategies;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Strategies;

public sealed class HttpRetryStrategySpecifications
{
    [Fact]
    public void Create_WithLogger_ReturnsHttpRetryStrategyOptions()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.Should().NotBeNull();
        result.Should().BeOfType<HttpRetryStrategyOptions>();
    }

    [Fact]
    public void Create_WithLogger_SetsMaxRetryAttemptsToThree()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.MaxRetryAttempts.Should().Be(3);
    }

    [Fact]
    public void Create_WithLogger_SetsInitialDelayToTwoSeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.Delay.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_WithLogger_SetsExponentialBackoffType()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.BackoffType.Should().Be(DelayBackoffType.Exponential);
    }

    [Fact]
    public void Create_WithLogger_EnablesJitter()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.UseJitter.Should().BeTrue();
    }

    [Fact]
    public void Create_WithLogger_OnRetryDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object);

        result.OnRetry.Should().NotBeNull();
    }

    [Fact]
    public async Task OnRetry_WhenOutcomeHasHttpResponse_LogsWarning()
    {
        var loggerMock = new Mock<ILogger>();
        var options = HttpRetryStrategy.Create(loggerMock.Object);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("service unavailable")
        };
        var context = ResilienceContextPool.Shared.Get(TestContext.Current.CancellationToken);
        var outcome = Outcome.FromResult<HttpResponseMessage>(httpResponse);
        var args = new OnRetryArguments<HttpResponseMessage>(context, outcome, 1, TimeSpan.FromSeconds(2), TimeSpan.Zero);

        await options.OnRetry!(args);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning
                , It.IsAny<EventId>()
                , It.IsAny<It.IsAnyType>()
                , It.IsAny<Exception?>()
                , It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
    }

    [Fact]
    public async Task OnRetry_WhenOutcomeIsNetworkFailure_LogsWarning()
    {
        var loggerMock = new Mock<ILogger>();
        var options = HttpRetryStrategy.Create(loggerMock.Object);
        var context = ResilienceContextPool.Shared.Get(TestContext.Current.CancellationToken);
        var outcome = Outcome.FromException<HttpResponseMessage>(new HttpRequestException("Network failure"));
        var args = new OnRetryArguments<HttpResponseMessage>(context, outcome, 1, TimeSpan.FromSeconds(2), TimeSpan.Zero);

        await options.OnRetry!(args);

        loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning
                , It.IsAny<EventId>()
                , It.IsAny<It.IsAnyType>()
                , It.IsAny<Exception?>()
                , It.IsAny<Func<It.IsAnyType, Exception?, string>>())
            , Times.Once);
    }
}
