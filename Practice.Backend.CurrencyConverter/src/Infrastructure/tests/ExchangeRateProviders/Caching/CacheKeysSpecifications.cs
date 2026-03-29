using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public sealed class CacheKeysSpecifications
{
    [Fact]
    public void Latest_Always_ReturnsKeyWithCorrectPrefix()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().StartWith(CacheKeys.Prefix);
    }

    [Fact]
    public void Latest_Always_ReturnsLowercaseKey()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().Be(key.ToLower());
    }

    [Fact]
    public void Latest_Always_ContainsProviderName()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().Contain(provider.Name.ToLower());
    }

    [Fact]
    public void Latest_Always_ContainsBaseCurrencyValue()
    {
        var baseCurrency = Currency.Create("USD");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().Contain(baseCurrency.Value.ToLower());
    }

    [Fact]
    public void Latest_Always_ContainsLatestSegment()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().Contain(":latest:");
    }

    [Fact]
    public void Latest_Always_ReturnsExpectedFormat()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Latest(baseCurrency, provider);

        key.Should().Be($"fx:currency-converter:frankfurter:latest:eur");
    }

    [Fact]
    public void Historical_Always_ReturnsKeyWithCorrectPrefix()
    {
        var baseCurrency = Currency.Create("EUR");
        var from = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Historical(baseCurrency, from, to, provider);

        key.Should().StartWith(CacheKeys.Prefix);
    }

    [Fact]
    public void Historical_Always_ReturnsLowercaseKey()
    {
        var baseCurrency = Currency.Create("EUR");
        var from = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Historical(baseCurrency, from, to, provider);

        key.Should().Be(key.ToLower());
    }

    [Fact]
    public void Historical_Always_ContainsHistoricalSegment()
    {
        var baseCurrency = Currency.Create("EUR");
        var from = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Historical(baseCurrency, from, to, provider);

        key.Should().Contain(":historical:");
    }

    [Fact]
    public void Historical_Always_ContainsFromAndToValues()
    {
        var baseCurrency = Currency.Create("EUR");
        var from = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Historical(baseCurrency, from, to, provider);

        key.Should().Contain(from.Value.ToString("O").ToLower().Replace("-", string.Empty, StringComparison.Ordinal).Substring(0, 4));
        key.Should().Contain(to.Value.ToString("O").ToLower().Replace("-", string.Empty, StringComparison.Ordinal).Substring(0, 4));
    }

    [Fact]
    public void Historical_Always_ReturnsExpectedFormat()
    {
        var baseCurrency = Currency.Create("EUR");
        var from = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));
        var provider = ExchangeRateProvider.Frankfurter;

        var key = CacheKeys.Historical(baseCurrency, from, to, provider);

        key.Should().Be($"fx:currency-converter:frankfurter:historical:eur:{from.Value}:{to.Value}".ToLower());
    }

    [Fact]
    public void Latest_DifferentCurrencies_ReturnsDifferentKeys()
    {
        var eur = Currency.Create("EUR");
        var usd = Currency.Create("USD");
        var provider = ExchangeRateProvider.Frankfurter;

        var eurKey = CacheKeys.Latest(eur, provider);
        var usdKey = CacheKeys.Latest(usd, provider);

        eurKey.Should().NotBe(usdKey);
    }

    [Fact]
    public void Historical_DifferentDateRanges_ReturnsDifferentKeys()
    {
        var baseCurrency = Currency.Create("EUR");
        var provider = ExchangeRateProvider.Frankfurter;
        var from1 = ExchangeDate.Create(new DateOnly(2024, 1, 1));
        var from2 = ExchangeDate.Create(new DateOnly(2024, 2, 1));
        var to = ExchangeDate.Create(new DateOnly(2024, 1, 15));

        var key1 = CacheKeys.Historical(baseCurrency, from1, to, provider);
        var key2 = CacheKeys.Historical(baseCurrency, from2, to, provider);

        key1.Should().NotBe(key2);
    }
}
