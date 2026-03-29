using System.Text.Json;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public sealed class CurrencyJsonConverterSpecifications
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new CurrencyJsonConverter() }
    };

    [Fact]
    public void Read_ValidCurrencyString_ReturnsCurrencyWithCorrectValue()
    {
        var json = "\"EUR\"";

        var result = JsonSerializer.Deserialize<Currency>(json, _options);

        result!.Value.Should().Be("EUR");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    public void Read_ValidCurrencyCode_DeserializesSuccessfully(string code)
    {
        var json = $"\"{code}\"";

        var result = JsonSerializer.Deserialize<Currency>(json, _options);

        result!.Value.Should().Be(code);
    }

    [Fact]
    public void Write_ValidCurrency_SerializesValueAsString()
    {
        var currency = Currency.Create("EUR");

        var json = JsonSerializer.Serialize(currency, _options);

        json.Should().Be("\"EUR\"");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    public void Write_ValidCurrencyCode_SerializesToExpectedJson(string code)
    {
        var currency = Currency.Create(code);

        var json = JsonSerializer.Serialize(currency, _options);

        json.Should().Be($"\"{code}\"");
    }

    [Fact]
    public void ReadAsPropertyName_ValidCurrencyKey_ReturnsCurrencyWithCorrectValue()
    {
        var json = """{"EUR":1.08}""";

        var result = JsonSerializer.Deserialize<Dictionary<Currency, double>>(json, _options);

        result!.Should().ContainKey(Currency.Create("EUR"));
    }

    [Fact]
    public void WriteAsPropertyName_ValidCurrency_SerializesAsDictionaryKey()
    {
        var dict = new Dictionary<Currency, double>
        {
            { Currency.Create("EUR"), 1.08 }
        };

        var json = JsonSerializer.Serialize(dict, _options);

        json.Should().Contain("\"EUR\"");
    }

    [Fact]
    public void RoundTrip_Currency_DeserializesToOriginalValue()
    {
        var original = Currency.Create("USD");

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<Currency>(json, _options);

        result!.Value.Should().Be(original.Value);
    }

    [Fact]
    public void RoundTrip_DictionaryWithCurrencyKey_DeserializesToOriginalValue()
    {
        var original = new Dictionary<Currency, decimal>
        {
            { Currency.Create("USD"), 1.08m },
            { Currency.Create("GBP"), 0.86m }
        };

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<Dictionary<Currency, decimal>>(json, _options);

        result!.Should().ContainKey(Currency.Create("USD"));
        result.Should().ContainKey(Currency.Create("GBP"));
    }
}
