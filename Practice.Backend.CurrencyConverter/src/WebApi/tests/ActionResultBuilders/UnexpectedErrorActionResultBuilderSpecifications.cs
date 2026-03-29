using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public partial class UnexpectedErrorActionResultBuilderSpecifications
{
    [Fact]
    public void CanHandle_ResultIsGeneric_ReturnsTrue()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic);

        builder.CanHandle(result).Should().BeTrue();
    }

    [Fact]
    public void CanHandle_ResultIsSuccess_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Success();

        builder.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void CanHandle_ResultIsNotAllowed_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotAllowed);

        builder.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void CanHandle_ResultIsNotFound_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

        builder.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void Build_GenericResult_ReturnsObjectResultWith500StatusCode()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.Generic,
            message: "An unexpected error occurred");

        var actionResult = (ObjectResult)builder.Build(result);

        actionResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void Build_GenericResult_ReturnsProblemDetailsWithUnexpectedErrorCode()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.Generic,
            message: "An unexpected error occurred");

        var actionResult = (ObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Extensions.Should().ContainKey(ExtensionKeys.Code);
        problemDetails.Extensions[ExtensionKeys.Code].Should().Be(ErrorCodes.UnexpectedError);
    }

    [Fact]
    public void Build_GenericResult_ReturnsProblemDetailsWithUnexpectedErrorTitle()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.Generic,
            message: "An unexpected error occurred");

        var actionResult = (ObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Title.Should().Be(ResponseTitles.UnexpectedError);
    }
}

public partial class UnexpectedErrorActionResultBuilderSpecifications
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
                .Returns(new ValidationProblemDetails());
        }

        public UnexpectedErrorActionResultBuilder Build()
            => new(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object);
    }
}
