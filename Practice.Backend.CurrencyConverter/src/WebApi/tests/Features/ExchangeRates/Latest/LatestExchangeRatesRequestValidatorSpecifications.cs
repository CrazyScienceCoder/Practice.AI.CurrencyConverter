using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Latest;

public sealed class LatestExchangeRatesRequestValidatorSpecifications
{
    private readonly LatestExchangeRatesRequestValidator _sut = new();

    [Fact]
    public async Task Validate_ValidRequestWithNullProvider_ReturnsValid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidRequestWithExplicitProvider_ReturnsValid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "Frankfurter" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyBaseCurrency_ReturnsInvalid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "", Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LatestExchangeRatesRequest.BaseCurrency));
    }

    [Fact]
    public async Task Validate_EmptyBaseCurrency_ReturnsRequiredMessage()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "", Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(LatestExchangeRatesRequest.BaseCurrency) &&
            e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task Validate_InvalidIsoCodeForBaseCurrency_ReturnsInvalid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "INVALID", Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(LatestExchangeRatesRequest.BaseCurrency) &&
            e.ErrorMessage.Contains("ISO 4217"));
    }

    [Fact]
    public async Task Validate_InvalidProvider_ReturnsInvalid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = "UnknownProvider" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(LatestExchangeRatesRequest.Provider));
    }

    [Fact]
    public async Task Validate_ProviderCaseInsensitive_ReturnsValid()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "USD", Provider = "frankfurter" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("JPY")]
    public async Task Validate_KnownIso4217Codes_ReturnsValid(string currency)
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = currency, Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }
}
