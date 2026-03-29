using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Logging;

public sealed class HttpContextEnricherSpecifications
{
    [Fact]
    public void Enrich_WhenHttpContextIsNull_DoesNotAddAnyProperties()
    {
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

        var sut = new HttpContextEnricher(httpContextAccessorMock.Object);
        var (logEvent, propertyFactoryMock) = BuildLogEvent();

        sut.Enrich(logEvent, propertyFactoryMock.Object);

        propertyFactoryMock.Verify(
            f => f.CreateProperty(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public void Enrich_WhenRemoteIpAddressIsPresent_AddsClientIPProperty()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("192.168.1.1");

        var (logEvent, propertyFactoryMock) = BuildLogEventWithPropertyFactory();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, propertyFactoryMock.Object);

        propertyFactoryMock.Verify(
            f => f.CreateProperty("ClientIP", It.IsAny<object?>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public void Enrich_WhenRemoteIpAddressIsNull_AddsClientIPWithUnknown()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = null;

        var (logEvent, _) = BuildLogEvent();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, new DirectPropertyFactory());

        logEvent.Properties.Should().ContainKey("ClientIP");
        logEvent.Properties["ClientIP"].ToString().Should().Contain("unknown");
    }

    [Fact]
    public void Enrich_WhenSubClaimIsPresent_AddsClientIdProperty()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user-42")]));

        var (logEvent, propertyFactoryMock) = BuildLogEventWithPropertyFactory();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, propertyFactoryMock.Object);

        propertyFactoryMock.Verify(
            f => f.CreateProperty("ClientId", It.IsAny<object?>(), It.IsAny<bool>()),
            Times.Once);
    }

    [Fact]
    public void Enrich_WhenSubClaimIsMissing_AddsClientIdWithAnonymous()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity());

        var (logEvent, _) = BuildLogEvent();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, new DirectPropertyFactory());

        logEvent.Properties.Should().ContainKey("ClientId");
        logEvent.Properties["ClientId"].ToString().Should().Contain("anonymous");
    }

    [Fact]
    public void Enrich_WhenHttpContextIsPresent_LogEventContainsCorrectClientIP()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.0.0.1");

        var (logEvent, _) = BuildLogEvent();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, new DirectPropertyFactory());

        logEvent.Properties.Should().ContainKey("ClientIP");
        logEvent.Properties["ClientIP"].ToString().Should().Contain("10.0.0.1");
    }

    [Fact]
    public void Enrich_WhenHttpContextIsPresent_LogEventContainsCorrectClientId()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user-99")]));

        var (logEvent, _) = BuildLogEvent();
        var sut = new HttpContextEnricher(BuildAccessor(context));

        sut.Enrich(logEvent, new DirectPropertyFactory());

        logEvent.Properties.Should().ContainKey("ClientId");
        logEvent.Properties["ClientId"].ToString().Should().Contain("user-99");
    }

    [Fact]
    public void Enrich_WhenPropertyAlreadyExists_DoesNotOverwrite()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("1.2.3.4");

        var factory = new DirectPropertyFactory();
        var existingProperty = factory.CreateProperty("ClientIP", "already-set");
        var messageTemplate = new MessageTemplateParser().Parse(string.Empty);
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            messageTemplate: messageTemplate,
            properties: [existingProperty]);

        var sut = new HttpContextEnricher(BuildAccessor(context));
        sut.Enrich(logEvent, factory);

        logEvent.Properties["ClientIP"].ToString().Should().Contain("already-set");
    }

    private static IHttpContextAccessor BuildAccessor(HttpContext context)
    {
        var mock = new Mock<IHttpContextAccessor>();
        mock.Setup(a => a.HttpContext).Returns(context);
        return mock.Object;
    }

    private static (LogEvent logEvent, Mock<ILogEventPropertyFactory> propertyFactoryMock) BuildLogEvent()
    {
        var messageTemplate = new MessageTemplateParser().Parse(string.Empty);
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            messageTemplate: messageTemplate,
            properties: []);

        var propertyFactoryMock = new Mock<ILogEventPropertyFactory>();
        return (logEvent, propertyFactoryMock);
    }

    private static (LogEvent logEvent, Mock<ILogEventPropertyFactory> propertyFactoryMock) BuildLogEventWithPropertyFactory()
    {
        var messageTemplate = new MessageTemplateParser().Parse(string.Empty);
        var logEvent = new LogEvent(
            DateTimeOffset.UtcNow,
            LogEventLevel.Information,
            exception: null,
            messageTemplate: messageTemplate,
            properties: []);

        var propertyFactoryMock = new Mock<ILogEventPropertyFactory>();
        propertyFactoryMock
            .Setup(f => f.CreateProperty(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<bool>()))
            .Returns((string name, object? value, bool _) =>
                new LogEventProperty(name, new ScalarValue(value)));

        return (logEvent, propertyFactoryMock);
    }

    private sealed class DirectPropertyFactory : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
            => new(name, new ScalarValue(value));
    }
}
