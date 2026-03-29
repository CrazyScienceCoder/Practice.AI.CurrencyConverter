using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.GetLatest;

public partial class GetLatestExchangeRateQueryHandlerSpecifications
{
    [Fact]
    public async Task Handle_SuccessfulRetrieval_ReturnsSuccessResponse()
    {
        var testBuilder = new TestBuilder()
            .SetupSuccess();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Base.Should().Be("USD");
        result.Data.Amount.Should().Be(1m);
    }

    [Fact]
    public async Task Handle_SuccessfulRetrieval_FiltersForbiddenCurrenciesFromRates()
    {
        var testBuilder = new TestBuilder()
            .SetupSuccessWithForbiddenCurrencies();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Rates.Should().ContainKey("EUR");
        result.Data.Rates.Should().NotContainKey("MXN");
        result.Data.Rates.Should().NotContainKey("PLN");
    }

    [Fact]
    public async Task Handle_ProviderReturnsNotFoundError_ReturnsNotFoundFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupNotFoundError();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Message.Should().Contain("USD");
    }

    [Fact]
    public async Task Handle_ProviderReturnsGenericError_ReturnsGenericFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupGenericError();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Generic);
        result.Message.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_DomainValidationExceptionThrown_ReturnsValidationErrorFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupDomainValidationException();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.ValidationError);
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_UnexpectedExceptionThrown_ReturnsGenericFailure()
    {
        var testBuilder = new TestBuilder()
            .SetupUnexpectedException();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.Generic);
        result.Error.Should().NotBeNull();
    }
}
