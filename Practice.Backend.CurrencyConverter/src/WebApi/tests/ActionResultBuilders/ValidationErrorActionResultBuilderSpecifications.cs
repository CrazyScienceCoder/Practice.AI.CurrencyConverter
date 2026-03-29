using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.WebApi.Constants;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public partial class ValidationErrorActionResultBuilderSpecifications
{
    [Fact]
    public void CanHandle_ResultIsValidationError_ReturnsTrue()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.ValidationError);

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
    public void CanHandle_ResultIsNotAllowed_ReturnsFalse()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotAllowed);

        builder.CanHandle(result).Should().BeFalse();
    }

    [Fact]
    public void Build_WithDomainValidationException_ReturnsBadRequestObjectResult()
    {
        var builder = new TestBuilder().Build();
        var exception = new InvalidCurrencyCodeException("Invalid currency code", "BaseCurrency");
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: exception,
            errorType: ErrorType.ValidationError);

        var actionResult = builder.Build(result);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Build_WithDomainValidationException_ReturnsValidationProblemDetailsWithDomainValidationCode()
    {
        var builder = new TestBuilder().Build();
        var exception = new InvalidCurrencyCodeException("Invalid currency code", "BaseCurrency");
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: exception,
            errorType: ErrorType.ValidationError);

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ValidationProblemDetails)actionResult.Value!;

        problemDetails.Extensions.Should().ContainKey(ExtensionKeys.Code);
        problemDetails.Extensions[ExtensionKeys.Code].Should().Be(ErrorCodes.DomainValidationFailed);
    }

    [Fact]
    public void Build_WithDomainValidationException_ErrorsContainParamName()
    {
        var builder = new TestBuilder().Build();
        var exception = new InvalidCurrencyCodeException("Invalid currency code", "BaseCurrency");
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: exception,
            errorType: ErrorType.ValidationError);

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ValidationProblemDetails)actionResult.Value!;

        problemDetails.Errors.Should().ContainKey("BaseCurrency");
    }

    [Fact]
    public void Build_WithGenericException_ReturnsBadRequestObjectResult()
    {
        var builder = new TestBuilder().Build();
        var exception = new Exception("Generic validation error");
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: exception,
            errorType: ErrorType.ValidationError);

        var actionResult = builder.Build(result);

        actionResult.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public void Build_WithGenericException_ReturnsValidationProblemDetailsWithGeneralValidationCode()
    {
        var builder = new TestBuilder().Build();
        var exception = new Exception("Generic validation error");
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: exception,
            errorType: ErrorType.ValidationError);

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ValidationProblemDetails)actionResult.Value!;

        problemDetails.Extensions.Should().ContainKey(ExtensionKeys.Code);
        problemDetails.Extensions[ExtensionKeys.Code].Should().Be(ErrorCodes.GeneralValidationFailed);
    }

    [Fact]
    public void Build_WithNullError_ReturnsValidationProblemDetailsWithNonFieldError()
    {
        var builder = new TestBuilder().Build();
        var result = GetLatestExchangeRateQueryResponse.Failure(
            exception: null,
            errorType: ErrorType.ValidationError,
            message: Constants.Constants.ValidationError);

        var actionResult = (BadRequestObjectResult)builder.Build(result);
        var problemDetails = (ValidationProblemDetails)actionResult.Value!;

        problemDetails.Errors.Should().ContainKey(Constants.Constants.NonFieldError);
    }
}
