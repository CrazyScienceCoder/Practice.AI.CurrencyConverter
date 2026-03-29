using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Filters;

public partial class ExceptionFilterSpecifications
{
    [Fact]
    public void OnException_UnhandledException_SetsExceptionHandledToTrue()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var context = testBuilder.BuildExceptionContext(new InvalidOperationException("test error"));

        filter.OnException(context);

        context.ExceptionHandled.Should().BeTrue();
    }

    [Fact]
    public void OnException_UnhandledException_SetsResultToObjectResult()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var context = testBuilder.BuildExceptionContext(new InvalidOperationException("test error"));

        filter.OnException(context);

        context.Result.Should().BeOfType<ObjectResult>();
    }

    [Fact]
    public void OnException_UnhandledException_ResultStatusCodeIs500()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var context = testBuilder.BuildExceptionContext(new InvalidOperationException("test error"));

        filter.OnException(context);

        var objectResult = (ObjectResult)context.Result!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void OnException_UnhandledException_ProblemDetailsHasUnexpectedErrorTitle()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var context = testBuilder.BuildExceptionContext(new Exception("something broke"));

        filter.OnException(context);

        var objectResult = (ObjectResult)context.Result!;
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Title.Should().Be(ResponseTitles.UnexpectedError);
    }

    [Fact]
    public void OnException_UnhandledException_ProblemDetailsDetailContainsExceptionMessage()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var exceptionMessage = "something went terribly wrong";
        var context = testBuilder.BuildExceptionContext(new Exception(exceptionMessage));

        filter.OnException(context);

        var objectResult = (ObjectResult)context.Result!;
        var problemDetails = (ProblemDetails)objectResult.Value!;
        problemDetails.Detail.Should().Be(exceptionMessage);
    }

    [Fact]
    public void OnException_UnhandledException_LogsErrorWithExceptionMessage()
    {
        var testBuilder = new TestBuilder();
        var filter = testBuilder.Build();
        var exception = new Exception("logged error");
        var context = testBuilder.BuildExceptionContext(exception);

        filter.OnException(context);

        testBuilder.LoggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("logged error")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
