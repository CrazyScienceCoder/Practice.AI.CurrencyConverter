using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Builders;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.ActionResultBuilders;

public sealed class ActionResultBuilderFactorySpecifications
{
    private readonly ActionResultBuilderFactory _sut;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
    private readonly Mock<ProblemDetailsFactory> _problemDetailsFactoryMock = new();

    public ActionResultBuilderFactorySpecifications()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Path = "/api/v1/test";

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _problemDetailsFactoryMock
            .Setup(x => x.CreateProblemDetails(
                It.IsAny<HttpContext>(), It.IsAny<int?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(new ProblemDetails());

        _problemDetailsFactoryMock
            .Setup(x => x.CreateValidationProblemDetails(
                It.IsAny<HttpContext>(), It.IsAny<ModelStateDictionary>(), It.IsAny<int?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .Returns(new ValidationProblemDetails());

        IActionResultBuilder[] builders =
        [
            new SuccessActionResultBuilder(),
            new NotAllowedActionResultBuilder(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object),
            new NotFoundActionResultBuilder(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object),
            new ValidationErrorActionResultBuilder(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object),
            new UnexpectedErrorActionResultBuilder(_httpContextAccessorMock.Object, _problemDetailsFactoryMock.Object)
        ];

        _sut = new ActionResultBuilderFactory(builders);
    }

    [Fact]
    public void Create_ResultIsSuccess_ReturnsSuccessBuilder()
    {
        var result = GetLatestExchangeRateQueryResponse.Success();

        var builder = _sut.Create(result);

        builder.Should().BeOfType<SuccessActionResultBuilder>();
    }

    [Fact]
    public void Create_ResultIsNotAllowed_ReturnsNotAllowedBuilder()
    {
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotAllowed);

        var builder = _sut.Create(result);

        builder.Should().BeOfType<NotAllowedActionResultBuilder>();
    }

    [Fact]
    public void Create_ResultIsNotFound_ReturnsNotFoundBuilder()
    {
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound);

        var builder = _sut.Create(result);

        builder.Should().BeOfType<NotFoundActionResultBuilder>();
    }

    [Fact]
    public void Create_ResultIsValidationError_ReturnsValidationErrorBuilder()
    {
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.ValidationError);

        var builder = _sut.Create(result);

        builder.Should().BeOfType<ValidationErrorActionResultBuilder>();
    }

    [Fact]
    public void Create_ResultIsGeneric_ReturnsUnexpectedErrorBuilder()
    {
        var result = GetLatestExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic);

        var builder = _sut.Create(result);

        builder.Should().BeOfType<UnexpectedErrorActionResultBuilder>();
    }

    [Fact]
    public void Create_NoMatchingBuilder_ThrowsInvalidOperationException()
    {
        var factory = new ActionResultBuilderFactory([]);
        var result = GetLatestExchangeRateQueryResponse.Success();

        var act = () => factory.Create(result);

        act.Should().ThrowExactly<InvalidOperationException>();
    }
}
