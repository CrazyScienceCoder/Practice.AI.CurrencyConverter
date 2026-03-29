using System.Text.Json;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public sealed class AmountJsonConverterSpecifications
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new AmountJsonConverter() }
    };

    [Fact]
    public void Read_ValidPositiveDecimal_ReturnsAmountWithCorrectValue()
    {
        var json = "1.08";

        var result = JsonSerializer.Deserialize<Amount>(json, _options);

        result!.Value.Should().Be(1.08m);
    }

    [Theory]
    [InlineData("0.01", 0.01)]
    [InlineData("1", 1)]
    [InlineData("100.50", 100.50)]
    public void Read_ValidAmountValues_DeserializesCorrectly(string json, decimal expectedValue)
    {
        var result = JsonSerializer.Deserialize<Amount>(json, _options);

        result!.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void Write_ValidAmount_SerializesValueAsNumber()
    {
        var amount = Amount.Create(1.08m);

        var json = JsonSerializer.Serialize(amount, _options);

        json.Should().Be("1.08");
    }

    [Theory]
    [InlineData(1.0, "1")]
    [InlineData(100.50, "100.5")]
    public void Write_ValidAmountValues_SerializesToExpectedJson(double value, string expectedJson)
    {
        var amount = Amount.Create((decimal)value);

        var json = JsonSerializer.Serialize(amount, _options);

        json.Should().Be(expectedJson);
    }

    [Fact]
    public void RoundTrip_Amount_DeserializesToOriginalValue()
    {
        var original = Amount.Create(99.99m);

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<Amount>(json, _options);

        result!.Value.Should().Be(original.Value);
    }
}
