using Microsoft.Extensions.Diagnostics.HealthChecks;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.HealthChecks;
using StackExchange.Redis;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.HealthChecks;

public sealed class RedisHealthCheckSpecifications
{
    [Fact]
    public async Task CheckHealthAsync_WhenPingSucceeds_ReturnsHealthy()
    {
        var sut = BuildSut(latency: TimeSpan.FromMilliseconds(3));

        var result = await sut.CheckHealthAsync(BuildContext(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(HealthStatus.Healthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPingSucceeds_DescriptionContainsLatency()
    {
        var sut = BuildSut(latency: TimeSpan.FromMilliseconds(7));

        var result = await sut.CheckHealthAsync(BuildContext(), TestContext.Current.CancellationToken);

        result.Description.Should().Contain("7");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPingThrows_ReturnsUnhealthy()
    {
        var db = new Mock<IDatabase>();
        db.Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "down"));

        var multiplexer = new Mock<IConnectionMultiplexer>();
        multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(db.Object);

        var sut = new RedisHealthCheck(multiplexer.Object);

        var result = await sut.CheckHealthAsync(BuildContext(), TestContext.Current.CancellationToken);

        result.Status.Should().Be(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPingThrows_DescriptionIndicatesUnreachable()
    {
        var db = new Mock<IDatabase>();
        db.Setup(d => d.PingAsync(It.IsAny<CommandFlags>()))
            .ThrowsAsync(new Exception("timeout"));

        var multiplexer = new Mock<IConnectionMultiplexer>();
        multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(db.Object);

        var sut = new RedisHealthCheck(multiplexer.Object);

        var result = await sut.CheckHealthAsync(BuildContext(), TestContext.Current.CancellationToken);

        result.Description.Should().Contain("unreachable");
    }

    [Fact]
    public async Task CheckHealthAsync_WhenPingThrows_ExceptionIsAttached()
    {
        var exception = new Exception("timeout");
        var db = new Mock<IDatabase>();
        db.Setup(d => d.PingAsync(It.IsAny<CommandFlags>())).ThrowsAsync(exception);

        var multiplexer = new Mock<IConnectionMultiplexer>();
        multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(db.Object);

        var sut = new RedisHealthCheck(multiplexer.Object);

        var result = await sut.CheckHealthAsync(BuildContext(), TestContext.Current.CancellationToken);

        result.Exception.Should().Be(exception);
    }

    private static RedisHealthCheck BuildSut(TimeSpan latency)
    {
        var db = new Mock<IDatabase>();
        db.Setup(d => d.PingAsync(It.IsAny<CommandFlags>())).ReturnsAsync(latency);

        var multiplexer = new Mock<IConnectionMultiplexer>();
        multiplexer.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(db.Object);

        return new RedisHealthCheck(multiplexer.Object);
    }

    private static HealthCheckContext BuildContext()
    {
        var registration = new HealthCheckRegistration("redis", Mock.Of<IHealthCheck>(), null, null);
        return new HealthCheckContext { Registration = registration };
    }
}
