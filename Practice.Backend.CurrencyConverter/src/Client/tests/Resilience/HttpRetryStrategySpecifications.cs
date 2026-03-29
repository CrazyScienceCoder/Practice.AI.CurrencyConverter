using System.Net;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Practice.Backend.CurrencyConverter.Client.Configuration;
using Practice.Backend.CurrencyConverter.Client.Resilience;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Resilience;

public sealed class HttpRetryStrategySpecifications
{
    private static CurrencyConverterClientOptions.RetryOptions DefaultOptions => new();

    [Fact]
    public void Create_WithOptions_ReturnsHttpRetryStrategyOptions()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.Should().NotBeNull();
        result.Should().BeOfType<HttpRetryStrategyOptions>();
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsMaxRetryAttemptsToThree()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.MaxRetryAttempts.Should().Be(3);
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsInitialDelayToTwoSeconds()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.Delay.Should().Be(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Create_WithDefaultOptions_SetsExponentialBackoffType()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.BackoffType.Should().Be(DelayBackoffType.Exponential);
    }

    [Fact]
    public void Create_WithDefaultOptions_EnablesJitter()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.UseJitter.Should().BeTrue();
    }

    [Fact]
    public void Create_WithDefaultOptions_OnRetryDelegateIsNotNull()
    {
        var loggerMock = new Mock<ILogger>();

        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        result.OnRetry.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Create_WithCustomMaxAttempts_SetsCorrectMaxRetryAttempts(int maxAttempts)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.RetryOptions { MaxAttempts = maxAttempts };

        var result = HttpRetryStrategy.Create(loggerMock.Object, options);

        result.MaxRetryAttempts.Should().Be(maxAttempts);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(30)]
    public void Create_WithCustomInitialDelay_SetsCorrectDelay(int initialDelaySeconds)
    {
        var loggerMock = new Mock<ILogger>();
        var options = new CurrencyConverterClientOptions.RetryOptions { InitialDelaySeconds = initialDelaySeconds };

        var result = HttpRetryStrategy.Create(loggerMock.Object, options);

        result.Delay.Should().Be(TimeSpan.FromSeconds(initialDelaySeconds));
    }

    [Fact]
    public async Task OnRetry_WhenOutcomeHasHttpResponse_LogsWarning()
    {
        var loggerMock = new Mock<ILogger>();
        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        var httpResponse = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent("service unavailable")
        };
        var outcome = Outcome.FromResult<HttpResponseMessage>(httpResponse);
        var context = ResilienceContextPool.Shared.Get();
        var args = new OnRetryArguments<HttpResponseMessage>(
            context, outcome, 1, TimeSpan.FromSeconds(1), TimeSpan.Zero);

        await result.OnRetry!(args);

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        ResilienceContextPool.Shared.Return(context);
    }

    [Fact]
    public async Task OnRetry_WhenOutcomeHasException_LogsWarning()
    {
        var loggerMock = new Mock<ILogger>();
        var result = HttpRetryStrategy.Create(loggerMock.Object, DefaultOptions);

        var outcome = Outcome.FromException<HttpResponseMessage>(new HttpRequestException("network error"));
        var context = ResilienceContextPool.Shared.Get();
        var args = new OnRetryArguments<HttpResponseMessage>(
            context, outcome, 1, TimeSpan.FromSeconds(1), TimeSpan.Zero);

        await result.OnRetry!(args);

        loggerMock.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        ResilienceContextPool.Shared.Return(context);
    }
}
