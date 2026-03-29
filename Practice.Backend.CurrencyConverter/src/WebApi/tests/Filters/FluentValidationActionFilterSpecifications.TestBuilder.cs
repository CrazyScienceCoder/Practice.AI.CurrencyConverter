using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Practice.Backend.CurrencyConverter.WebApi.Filters;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Filters;

public partial class FluentValidationActionFilterSpecifications
{
    private class TestBuilder
    {
        private readonly Mock<IServiceProvider> _serviceProviderMock = new();
        private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock = new();

        public TestBuilder()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(ProblemDetailsFactory)))
                .Returns(_problemDetailsFactoryMock.Object);

            _problemDetailsFactoryMock
                .Setup(x => x.CreateValidationProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<ModelStateDictionary>(),
                    It.IsAny<int?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>()))
                .Returns((HttpContext ctx, ModelStateDictionary msd, int? status, string? title, string? type, string? detail, string? instance) =>
                {
                    var details = new ValidationProblemDetails { Status = status ?? StatusCodes.Status400BadRequest };
                    foreach (var key in msd.Keys)
                    {
                        var errors = msd[key]?.Errors.Select(e => e.ErrorMessage).ToArray() ?? [];
                        details.Errors[key] = errors;
                    }
                    return details;
                });

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
                .Returns(null!);
        }

        public TestBuilder WithNoValidatorRegistered()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
                .Returns(null!);
            return this;
        }

        public TestBuilder WithValidValidator()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
                .Returns(new TestRequestValidator());
            return this;
        }

        public TestBuilder WithInvalidValidator()
        {
            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IValidator<TestRequest>)))
                .Returns(new TestRequestValidator());
            return this;
        }

        public (ActionExecutingContext context, NextCalledTracker nextCalled) BuildContextWithArguments(
            IEnumerable<KeyValuePair<string, object?>> arguments)
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProviderMock.Object
            };
            httpContext.Request.Path = "/api/v1/test";

            var actionContext = new ActionContext(
                httpContext,
                new RouteData(),
                new ActionDescriptor());

            var context = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                arguments.ToDictionary(kv => kv.Key, kv => kv.Value),
                new object());

            return (context, new NextCalledTracker());
        }

        public FluentValidationActionFilter Build() => new();
    }
}
