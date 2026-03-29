using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.WebApi.Filters;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Filters;

public partial class ExceptionFilterSpecifications
{
    private class TestBuilder
    {
        public readonly Mock<ILogger<ExceptionFilter>> LoggerMock = new();
        private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock = new();

        public TestBuilder()
        {
            _problemDetailsFactoryMock
                .Setup(x => x.CreateProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<int?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .Returns((HttpContext ctx, int? status, string? title, string? type, string? detail, string? instance) =>
                    new ProblemDetails { Status = status, Title = title, Detail = detail });

            _problemDetailsFactoryMock
                .Setup(x => x.CreateValidationProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<ModelStateDictionary>(),
                    It.IsAny<int?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .Returns(new ValidationProblemDetails());
        }

        public ExceptionContext BuildExceptionContext(Exception exception)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/v1/exchange-rate/latest";

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());

            return new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = exception
            };
        }

        public ExceptionFilter Build()
            => new(_problemDetailsFactoryMock.Object, LoggerMock.Object);
    }
}
