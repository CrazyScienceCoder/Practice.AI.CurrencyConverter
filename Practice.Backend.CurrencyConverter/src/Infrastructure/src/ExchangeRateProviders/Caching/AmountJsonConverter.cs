using System.Text.Json;
using System.Text.Json.Serialization;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

public sealed class AmountJsonConverter : JsonConverter<Amount>
{
    public override Amount Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetDecimal();
        return Amount.Create(value);
    }

    public override void Write(Utf8JsonWriter writer, Amount value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}