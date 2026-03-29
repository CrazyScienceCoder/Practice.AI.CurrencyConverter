using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Conversion;

namespace Practice.Backend.CurrencyConverter.WebApi.Tests.Features.ExchangeRates.Conversion;

public sealed class ConversionRequestValidatorSpecifications
{
    private readonly ConversionRequestValidator _sut = new();

    private static ConversionRequest BuildValidRequest()
        => new()
        {
            BaseCurrency = "USD",
            ToCurrency = "EUR",
            Amount = 100m,
            Provider = null
        };

    [Fact]
    public async Task Validate_ValidRequest_ReturnsValid()
    {
        var result = await _sut.ValidateAsync(BuildValidRequest(), TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_AmountBelowMinimum_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { Amount = 0m };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ConversionRequest.Amount));
    }

    [Fact]
    public async Task Validate_AmountAtMinimum_ReturnsValid()
    {
        var request = BuildValidRequest() with { Amount = 1m };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyBaseCurrency_ReturnsInvalidWithRequiredMessage()
    {
        var request = BuildValidRequest() with { BaseCurrency = "" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(ConversionRequest.BaseCurrency) &&
            e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task Validate_InvalidBaseCurrencyIsoCode_ReturnsInvalidWithIsoMessage()
    {
        var request = BuildValidRequest() with { BaseCurrency = "XYZ123" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(ConversionRequest.BaseCurrency) &&
            e.ErrorMessage.Contains("ISO 4217"));
    }

    [Fact]
    public async Task Validate_EmptyToCurrency_ReturnsInvalidWithRequiredMessage()
    {
        var request = BuildValidRequest() with { ToCurrency = "" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(ConversionRequest.ToCurrency) &&
            e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public async Task Validate_InvalidToCurrencyIsoCode_ReturnsInvalidWithIsoMessage()
    {
        var request = BuildValidRequest() with { ToCurrency = "INVALID" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(ConversionRequest.ToCurrency) &&
            e.ErrorMessage.Contains("ISO 4217"));
    }

    [Fact]
    public async Task Validate_NullProvider_ReturnsValid()
    {
        var request = BuildValidRequest() with { Provider = null };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_InvalidProvider_ReturnsInvalid()
    {
        var request = BuildValidRequest() with { Provider = "NonExistentProvider" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(ConversionRequest.Provider));
    }

    [Fact]
    public async Task Validate_ValidProviderCaseInsensitive_ReturnsValid()
    {
        var request = BuildValidRequest() with { Provider = "FRANKFURTER" };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999.99)]
    public async Task Validate_AmountAboveMinimum_ReturnsValid(decimal amount)
    {
        var request = BuildValidRequest() with { Amount = amount };

        var result = await _sut.ValidateAsync(request, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
    }
}
