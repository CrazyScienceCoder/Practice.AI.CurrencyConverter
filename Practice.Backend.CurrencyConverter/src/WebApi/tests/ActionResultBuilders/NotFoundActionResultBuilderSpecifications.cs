using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public partial class NotFoundActionResultBuilderSpecifications
{
    [Fact]
    public void CanHandle_ResultIsNotFound_ReturnsTrue()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

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
    public void CanHandle_ResultIsValidationError_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.ValidationError);

        builder.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void Build_NotFoundResult_ReturnsNotFoundObjectResult()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotFound,
            message: "Exchange rate not found for USD");

        var actionResult = builder.Build(result);

        actionResult.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public void Build_NotFoundResult_ReturnsProblemDetailsWithExchangeRateNotFoundCode()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotFound,
            message: "Exchange rate not found for USD");

        var actionResult = (NotFoundObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Extensions.Should().ContainKey(ExtensionKeys.Code);
        problemDetails.Extensions[ExtensionKeys.Code].Should().Be(ErrorCodes.ExchangeRateNotFound);
    }

    [Fact]
    public void Build_NotFoundResult_ReturnsProblemDetailsWithNotFoundTitle()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotFound,
            message: "Exchange rate not found for USD");

        var actionResult = (NotFoundObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Title.Should().Be(ResponseTitles.ExchangeRateNotFound);
    }
}
