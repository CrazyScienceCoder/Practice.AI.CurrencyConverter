using System.Text.Json;
using System.Text.Json.Serialization;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

public sealed class CurrencyJsonConverter : JsonConverter<Currency>
{
    public override Currency Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Currency.Create(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }

    public override Currency ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return Currency.Create(reader.GetString()!);
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, Currency value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.Value);
    }
}