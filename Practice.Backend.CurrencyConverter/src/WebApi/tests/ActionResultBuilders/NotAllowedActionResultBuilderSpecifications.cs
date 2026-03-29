using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public partial class NotAllowedActionResultBuilderSpecifications
{
    [Fact]
    public void CanHandle_ResultIsNotAllowed_ReturnsTrue()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotAllowed);

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
    public void CanHandle_ResultIsNotFound_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

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
    public void Build_NotAllowedResult_ReturnsBadRequestObjectResult()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotAllowed,
            message: "Currency TRY is not allowed");

        var actionResult = builder.Build(result);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Build_NotAllowedResult_ReturnsProblemDetailsWithCurrencyNotAllowedCode()
    {
        var testBuilder = new TestBuilder();
        var builder = testBuilder.Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotAllowed,
            message: "Currency TRY is not allowed");

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Extensions.Should().ContainKey(ExtensionKeys.Code);
        problemDetails.Extensions[ExtensionKeys.Code].Should().Be(ErrorCodes.CurrencyNotAllowed);
    }

    [Fact]
    public void Build_NotAllowedResult_ReturnsProblemDetailsWithBadRequestTitle()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            errorType: ErrorType.NotAllowed,
            message: "Currency TRY is not allowed");

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ProblemDetails)actionResult.Value!;

        problemDetails.Title.Should().Be(ResponseTitles.BadRequest);
    }
}
