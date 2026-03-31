using System.Diagnostics;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Logging;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Instrumentation.Logging;

public sealed class ActivityEnricherSpecifications
{
    private readonly ActivityEnricher _sut = new();

    [Fact]
    public void Enrich_WhenNoCurrentActivity_DoesNotAddAnyProperties()
    {
        Activity.Current = null;

        var (logEvent, propertyFactoryMock) = BuildLogEvent();

        _sut.Enrich(logEvent, propertyFactoryMock.Object);

        propertyFactoryMock.Verify(
            f => f.CreateProperty(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<bool>()),
            Times.Never);
    }

    [Fact]
    public void Enrich_WhenCurrentActivityExists_AddsTraceIdProperty()
    {
        var (logEvent, propertyFactoryMock) = BuildLogEventWithPropertyFactory();

        using var activity = new Activity("TestActivity");
        activity.Start();

        try
        {
            _sut.Enrich(logEvent, propertyFactoryMock.Object);

            propertyFactoryMock.Verify(
                f => f.CreateProperty("TraceId", It.IsAny<object?>(), It.IsAny<bool>()),
                Times.Once);
        }
        finally
        {
            activity.Stop();
        }
    }

    [Fact]
    public void Enrich_WhenCurrentActivityExists_AddsSpanIdProperty()
    {
        var (logEvent, propertyFactoryMock) = BuildLogEventWithPropertyFactory();

        using var activity = new Activity("TestActivity");
        activity.Start();

        try
        {
            _sut.Enrich(logEvent, propertyFactoryMock.Object);

            propertyFactoryMock.Verify(
                f => f.CreateProperty("SpanId", It.IsAny<object?>(), It.IsAny<bool>()),
                Times.Once);
        }
        finally
        {
            activity.Stop();
        }
    }

    [Fact]
    public void Enrich_WhenCurrentActivityExists_AddsParentSpanIdProperty()
    {
        var (logEvent, propertyFactoryMock) = BuildLogEventWithPropertyFactory();

        using var parentActivity = new Activity("ParentActivity");
        parentActivity.Start();

        using var childActivity = new Activity("ChildActivity");
        childActivity.Start();

        try
        {
            _sut.Enrich(logEvent, propertyFactoryMock.Object);

            propertyFactoryMock.Verify(
                f => f.CreateProperty("ParentSpanId", It.IsAny<object?>(), It.IsAny<bool>()),
                Times.Once);
        }
        finally
        {
            childActivity.Stop();
            parentActivity.Stop();
        }
    }

    [Fact]
    public void Enrich_WhenCurrentActivityExists_LogEventContainsTraceIdProperty()
    {
        using var activity = new Activity("TestActivity");
        activity.Start();

        try
        {
            var (logEvent, _) = BuildLogEvent();
            var traceId = activity.TraceId.ToString();

            _sut.Enrich(logEvent, new DirectPropertyFactory());

            logEvent.Properties.Should().ContainKey("TraceId");
            logEvent.Properties["TraceId"].ToString().Should().Contain(traceId);
        }
        finally
        {
            activity.Stop();
        }
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
