using Practice.Backend.CurrencyConverter.Client.Internal;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Internal;

public sealed class QueryBuilderSpecifications
{
    [Fact]
    public void ForLatest_WithBaseCurrency_SetsBaseCurrencyQueryParameter()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR" };

        var query = QueryBuilder.ForLatest(request);

        query.Should().ContainKey("BaseCurrency").WhoseValue.Should().Be("EUR");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    public void ForLatest_WithVariousBaseCurrencies_SetsCorrectBaseCurrency(string baseCurrency)
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = baseCurrency };

        var query = QueryBuilder.ForLatest(request);

        query["BaseCurrency"].Should().Be(baseCurrency);
    }

    [Fact]
    public void ForLatest_WithProvider_SetsProviderQueryParameter()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "frankfurter" };

        var query = QueryBuilder.ForLatest(request);

        query.Should().ContainKey("Provider").WhoseValue.Should().Be("frankfurter");
    }

    [Fact]
    public void ForLatest_WithNullProvider_DoesNotContainProviderKey()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = null };

        var query = QueryBuilder.ForLatest(request);

        query.Should().NotContainKey("Provider");
    }

    [Fact]
    public void ForLatest_WithWhitespaceProvider_DoesNotContainProviderKey()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR", Provider = "   " };

        var query = QueryBuilder.ForLatest(request);

        query.Should().NotContainKey("Provider");
    }

    [Fact]
    public void ForLatest_WithNoProvider_ContainsOnlyBaseCurrencyKey()
    {
        var request = new LatestExchangeRatesRequest { BaseCurrency = "EUR" };

        var query = QueryBuilder.ForLatest(request);

        query.Should().ContainSingle().Which.Key.Should().Be("BaseCurrency");
    }

    [Fact]
    public void ForHistorical_WithBaseCurrency_SetsBaseCurrencyQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("BaseCurrency").WhoseValue.Should().Be("EUR");
    }

    [Fact]
    public void ForHistorical_WithFromDate_SetsFromQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            From = new DateOnly(2024, 1, 1)
        };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("From");
    }

    [Fact]
    public void ForHistorical_WithFromDate_FormatsDateAsYearMonthDay()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            From = new DateOnly(2024, 3, 15)
        };

        var query = QueryBuilder.ForHistorical(request);

        query["From"].Should().Be("2024-03-15");
    }

    [Fact]
    public void ForHistorical_WithToDate_SetsToQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            To = new DateOnly(2024, 12, 31)
        };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("To");
    }

    [Fact]
    public void ForHistorical_WithToDate_FormatsDateAsYearMonthDay()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            To = new DateOnly(2024, 12, 31)
        };

        var query = QueryBuilder.ForHistorical(request);

        query["To"].Should().Be("2024-12-31");
    }

    [Fact]
    public void ForHistorical_WithPageNumber_SetsPageNumberQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", PageNumber = 2 };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("PageNumber").WhoseValue.Should().Be("2");
    }

    [Fact]
    public void ForHistorical_WithDaysPerPage_SetsDaysPerPageQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", DaysPerPage = 7 };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("DaysPerPage").WhoseValue.Should().Be("7");
    }

    [Fact]
    public void ForHistorical_WithNullFromDate_DoesNotContainFromKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", From = null };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().NotContainKey("From");
    }

    [Fact]
    public void ForHistorical_WithNullToDate_DoesNotContainToKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", To = null };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().NotContainKey("To");
    }

    [Fact]
    public void ForHistorical_WithNullPageNumber_DoesNotContainPageNumberKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", PageNumber = null };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().NotContainKey("PageNumber");
    }

    [Fact]
    public void ForHistorical_WithNullDaysPerPage_DoesNotContainDaysPerPageKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", DaysPerPage = null };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().NotContainKey("DaysPerPage");
    }

    [Fact]
    public void ForHistorical_WithProvider_SetsProviderQueryParameter()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", Provider = "frankfurter" };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("Provider").WhoseValue.Should().Be("frankfurter");
    }

    [Fact]
    public void ForHistorical_WithNullProvider_DoesNotContainProviderKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR", Provider = null };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().NotContainKey("Provider");
    }

    [Fact]
    public void ForHistorical_WithNoOptionalParameters_ContainsOnlyBaseCurrencyKey()
    {
        var request = new HistoricalExchangeRateRequest { BaseCurrency = "EUR" };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainSingle().Which.Key.Should().Be("BaseCurrency");
    }

    [Fact]
    public void ForHistorical_WithAllOptionalParameters_ContainsAllKeys()
    {
        var request = new HistoricalExchangeRateRequest
        {
            BaseCurrency = "EUR",
            From = new DateOnly(2024, 1, 1),
            To = new DateOnly(2024, 1, 31),
            PageNumber = 1,
            DaysPerPage = 7,
            Provider = "frankfurter"
        };

        var query = QueryBuilder.ForHistorical(request);

        query.Should().ContainKey("BaseCurrency");
        query.Should().ContainKey("From");
        query.Should().ContainKey("To");
        query.Should().ContainKey("PageNumber");
        query.Should().ContainKey("DaysPerPage");
        query.Should().ContainKey("Provider");
    }

    [Fact]
    public void ForConversion_WithBaseCurrency_SetsBaseCurrencyQueryParameter()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        var query = QueryBuilder.ForConversion(request);

        query.Should().ContainKey("BaseCurrency").WhoseValue.Should().Be("EUR");
    }

    [Fact]
    public void ForConversion_WithToCurrency_SetsToCurrencyQueryParameter()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        var query = QueryBuilder.ForConversion(request);

        query.Should().ContainKey("ToCurrency").WhoseValue.Should().Be("USD");
    }

    [Fact]
    public void ForConversion_WithAmount_SetsAmountQueryParameter()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        var query = QueryBuilder.ForConversion(request);

        query.Should().ContainKey("Amount");
    }

    [Fact]
    public void ForConversion_WithWholeAmount_FormatsAmountWithGSpecifier()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 100 };

        var query = QueryBuilder.ForConversion(request);

        query["Amount"].Should().Be("100");
    }

    [Fact]
    public void ForConversion_WithDecimalAmount_FormatsAmountWithGSpecifier()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 99.99m };

        var query = QueryBuilder.ForConversion(request);

        query["Amount"].Should().Be("99.99");
    }

    [Fact]
    public void ForConversion_WithProvider_SetsProviderQueryParameter()
    {
        var request = new ConversionRequest
        {
            BaseCurrency = "EUR",
            ToCurrency = "USD",
            Amount = 100,
            Provider = "frankfurter"
        };

        var query = QueryBuilder.ForConversion(request);

        query.Should().ContainKey("Provider").WhoseValue.Should().Be("frankfurter");
    }

    [Fact]
    public void ForConversion_WithNullProvider_DoesNotContainProviderKey()
    {
        var request = new ConversionRequest
        {
            BaseCurrency = "EUR",
            ToCurrency = "USD",
            Amount = 100,
            Provider = null
        };

        var query = QueryBuilder.ForConversion(request);

        query.Should().NotContainKey("Provider");
    }

    [Fact]
    public void ForConversion_WithAllParameters_ContainsBaseCurrencyToCurrencyAndAmount()
    {
        var request = new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = 50 };

        var query = QueryBuilder.ForConversion(request);

        query.Should().ContainKey("BaseCurrency");
        query.Should().ContainKey("ToCurrency");
        query.Should().ContainKey("Amount");
    }
}
