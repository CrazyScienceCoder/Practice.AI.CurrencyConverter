using Practice.Backend.CurrencyConverter.Application.Shared;

namespace Practice.Backend.CurrencyConverter.Application.Tests.ExchangeRates.GetHistorical;

public partial class GetHistoricalExchangeRateQueryHandlerSpecifications
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
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
    public async Task Handle_NullPageNumberAndDaysPerPage_UsesDefaultPaginationValues()
    {
        var testBuilder = new TestBuilder()
            .SetupSuccess();

        var handler = testBuilder.Build();

        // DefaultQuery has null PageNumber and DaysPerPage — handler uses defaults (page 1, 10 days/page)
        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data!.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task Handle_RequestedPageBeyondAvailablePages_ReturnsNotFoundFailure()
    {
        // 1 business day total, requesting page 2 → empty pageDates
        var testBuilder = new TestBuilder()
            .SetupPageOutOfRange();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.PageOutOfRangeQuery, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
        result.Message.Should().Contain("No valid business days");
    }

    [Fact]
    public async Task Handle_DateRangeCoveringOnlyWeekend_ReturnsNotFoundFailure()
    {
        var testBuilder = new TestBuilder();
        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.WeekendOnlyQuery, TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorType.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_MorePagesAvailable_SetsHasMoreToTrue()
    {
        // 10 business days, DaysPerPage = 5, PageNumber = 1 → hasMore = true
        var testBuilder = new TestBuilder()
            .SetupSuccessForPaginatedQuery();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.PaginatedQueryPage1, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data!.HasMore.Should().BeTrue();
        result.Data.TotalNumberOfPages.Should().Be(2);
        result.Data.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task Handle_OnLastPage_SetsHasMoreToFalse()
    {
        // 10 business days, DaysPerPage = 5, PageNumber = 2 → hasMore = false
        var testBuilder = new TestBuilder()
            .SetupSuccessForPaginatedQuery();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.PaginatedQueryPage2, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data!.HasMore.Should().BeFalse();
        result.Data.TotalNumberOfPages.Should().Be(2);
        result.Data.PageNumber.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SuccessfulRetrieval_FiltersForbiddenCurrenciesFromRates()
    {
        var testBuilder = new TestBuilder()
            .SetupSuccessWithForbiddenCurrencies();

        var handler = testBuilder.Build();

        var result = await handler.Handle(testBuilder.DefaultQuery, TestContext.Current.CancellationToken);

        testBuilder.ProviderMock.Verify();
        result.IsSuccess.Should().BeTrue();
        result.Data!.Rates.Values.Should().AllSatisfy(
            dailyRates =>
            {
                dailyRates.Should().NotContainKey("MXN");
                dailyRates.Should().NotContainKey("PLN");
            });
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
