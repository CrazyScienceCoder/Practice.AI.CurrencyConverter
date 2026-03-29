using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public partial class NotFoundActionResultBuilderSpecifications
{
    private class TestBuilder
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock = new();

        public TestBuilder()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Path = "/api/v1/exchange-rate/latest";

            _httpContextAccessorMock
                .Setup(x => x.HttpContext)
                .Returns(httpContext);

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
                .Returns((HttpContext ctx, ModelStateDictionary msd, int? status, string? title, string? type, string? detail, string? instance) =>
                    new ValidationProblemDetails { Status = status, Title = title, Detail = detail });
        }

        public NotFoundActionResultBuilder Build()
            => new(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object);
    }
}
