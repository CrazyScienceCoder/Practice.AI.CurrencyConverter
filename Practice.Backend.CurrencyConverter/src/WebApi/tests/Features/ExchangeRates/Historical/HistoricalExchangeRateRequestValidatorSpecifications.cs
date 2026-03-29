using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Historical;

public sealed class HistoricalExchangeRateRequestValidatorSpecifications
{
    private readonly HistoricalExchangeRateRequestValidator _sut = new();

    private static HistoricalExchangeRateRequest BuildValidRequest()
        => new()
        {
            BaseCurrency = "USD",
            From = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-10),
            To = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1),
            Provider = null
        };

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var request = BuildValidRequest();

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyBaseCurrency_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { BaseCurrency = "" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(HistoricalExchangeRateRequest.BaseCurrency));
    }

    [Fact]
    public async Task Validate_InvalidBaseCurrencyCode_ReturnsInvalidWithIso4217Message()
    {
        var request = BuildValidRequest() with { BaseCurrency = "INVALID" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(HistoricalExchangeRateRequest.BaseCurrency) &&
            e.ErrorMessage.Contains("ISO 4217"));
    }

    [Fact]
    public async Task Validate_NullFromDate_ReturnsInvalidWithRequiredMessage()
    {
        var request = BuildValidRequest() with { From = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(HistoricalExchangeRateRequest.From) &&
            e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task Validate_FutureFromDate_ReturnsInvalidWithFutureMessage()
    {
        var request = BuildValidRequest() with { From = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1) };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(HistoricalExchangeRateRequest.From) &&
            e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public async Task Validate_NullToDate_ReturnsInvalidWithRequiredMessage()
    {
        var request = BuildValidRequest() with { To = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(HistoricalExchangeRateRequest.To) &&
            e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task Validate_FutureToDate_ReturnsInvalidWithFutureMessage()
    {
        var request = BuildValidRequest() with { To = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1) };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(HistoricalExchangeRateRequest.To) &&
            e.ErrorMessage.Contains("future"));
    }

    [Fact]
    public async Task Validate_InvalidProvider_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { Provider = "UnknownProvider" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(HistoricalExchangeRateRequest.Provider));
    }

    [Fact]
    public async Task Validate_PageNumberZero_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { PageNumber = 0 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(HistoricalExchangeRateRequest.PageNumber));
    }

    [Fact]
    public async Task Validate_PageNumberOne_ReturnsValid()
    {
        var request = BuildValidRequest() with { PageNumber = 1 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_PageNumberNull_ReturnsValid()
    {
        var request = BuildValidRequest() with { PageNumber = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DaysPerPageZero_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { DaysPerPage = 0 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(HistoricalExchangeRateRequest.DaysPerPage));
    }

    [Fact]
    public async Task Validate_DaysPerPageExceedsMaximum_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { DaysPerPage = 31 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(HistoricalExchangeRateRequest.DaysPerPage));
    }

    [Fact]
    public async Task Validate_DaysPerPageAtMaximum_ReturnsValid()
    {
        var request = BuildValidRequest() with { DaysPerPage = 30 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DaysPerPageAtMinimum_ReturnsValid()
    {
        var request = BuildValidRequest() with { DaysPerPage = 1 };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DaysPerPageNull_ReturnsValid()
    {
        var request = BuildValidRequest() with { DaysPerPage = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_TodayFromDate_ReturnsValid()
    {
        var request = BuildValidRequest() with { From = DateOnly.FromDateTime(DateTime.UtcNow) };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }
}
