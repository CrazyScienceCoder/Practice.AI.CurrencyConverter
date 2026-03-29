using System.Text.Json;
using Practice.Backend.CurrencyConverter.Domain.Types;
using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Caching;

public sealed class ExchangeDateJsonConverterSpecifications
{
    private readonly JsonSerializerOptions _options = new()
    {
        Converters = { new ExchangeDateJsonConverter() }
    };

    [Fact]
    public void Read_ValidDateString_ReturnsExchangeDateWithCorrectValue()
    {
        const string json = "\"2024-01-15\"";

        var result = JsonSerializer.Deserialize<ExchangeDate>(json, _options);

        result!.Value.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Theory]
    [InlineData("\"2024-01-01\"", 2024, 1, 1)]
    [InlineData("\"2023-12-31\"", 2023, 12, 31)]
    [InlineData("\"2020-06-15\"", 2020, 6, 15)]
    public void Read_ValidDateStrings_DeserializesCorrectly(string json, int year, int month, int day)
    {
        var result = JsonSerializer.Deserialize<ExchangeDate>(json, _options);

        result!.Value.Should().Be(new DateOnly(year, month, day));
    }

    [Fact]
    public void Read_InvalidDateString_ThrowsJsonException()
    {
        const string json = "\"not-a-date\"";

        var act = () => JsonSerializer.Deserialize<ExchangeDate>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid ExchangeDate format*");
    }

    [Fact]
    public void Read_MalformedDateString_ThrowsJsonException()
    {
        // DateOnly.TryParse might succeed for slash-delimited; test a truly invalid value
        const string badJson = "\"abc-def-ghi\"";

        var act = () => JsonSerializer.Deserialize<ExchangeDate>(badJson, _options);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Write_ValidExchangeDate_SerializesInYyyyMmDdFormat()
    {
        var date = ExchangeDate.Create(new DateOnly(2024, 1, 15));

        var json = JsonSerializer.Serialize(date, _options);

        json.Should().Be("\"2024-01-15\"");
    }

    [Theory]
    [InlineData(2024, 1, 1, "\"2024-01-01\"")]
    [InlineData(2023, 12, 31, "\"2023-12-31\"")]
    [InlineData(2020, 6, 15, "\"2020-06-15\"")]
    public void Write_ValidExchangeDates_SerializesToExpectedFormat(int year, int month, int day, string expectedJson)
    {
        var date = ExchangeDate.Create(new DateOnly(year, month, day));

        var json = JsonSerializer.Serialize(date, _options);

        json.Should().Be(expectedJson);
    }

    [Fact]
    public void ReadAsPropertyName_ValidDateKey_ReturnsExchangeDateWithCorrectValue()
    {
        const string json = """{"2024-01-15":1.08}""";

        var result = JsonSerializer.Deserialize<Dictionary<ExchangeDate, double>>(json, _options);

        result!.Should().ContainKey(ExchangeDate.Create(new DateOnly(2024, 1, 15)));
    }

    [Fact]
    public void ReadAsPropertyName_InvalidDateKey_ThrowsJsonException()
    {
        const string json = """{"not-a-date":1.08}""";

        var act = () => JsonSerializer.Deserialize<Dictionary<ExchangeDate, double>>(json, _options);

        act.Should().Throw<JsonException>()
            .WithMessage("*Invalid ExchangeDate key format*");
    }

    [Fact]
    public void WriteAsPropertyName_ValidExchangeDate_SerializesAsDictionaryKey()
    {
        var dict = new Dictionary<ExchangeDate, double>
        {
            { ExchangeDate.Create(new DateOnly(2024, 1, 15)), 1.08 }
        };

        var json = JsonSerializer.Serialize(dict, _options);

        json.Should().Contain("\"2024-01-15\"");
    }

    [Fact]
    public void RoundTrip_ExchangeDate_DeserializesToOriginalValue()
    {
        var original = ExchangeDate.Create(new DateOnly(2024, 6, 20));

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<ExchangeDate>(json, _options);

        result!.Value.Should().Be(original.Value);
    }

    [Fact]
    public void RoundTrip_DictionaryWithExchangeDateKey_DeserializesToOriginalValue()
    {
        var original = new Dictionary<ExchangeDate, decimal>
        {
            { ExchangeDate.Create(new DateOnly(2024, 1, 1)), 1.08m },
            { ExchangeDate.Create(new DateOnly(2024, 1, 2)), 1.09m }
        };

        var json = JsonSerializer.Serialize(original, _options);
        var result = JsonSerializer.Deserialize<Dictionary<ExchangeDate, decimal>>(json, _options);

        result!.Should().ContainKey(ExchangeDate.Create(new DateOnly(2024, 1, 1)));
        result.Should().ContainKey(ExchangeDate.Create(new DateOnly(2024, 1, 2)));
    }
}
