using System.Text.Json;
using System.Text.Json.Serialization;
using Practice.Backend.CurrencyConverter.Domain.Types;

namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Caching;

//public class ExchangeDateJsonConverter : JsonConverter<ExchangeDate>
//{
//    public override ExchangeDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        var date = reader.GetDateTime();
//        return new ExchangeDate(DateOnly.FromDateTime(date));
//    }

//    public override void Write(Utf8JsonWriter writer, ExchangeDate value, JsonSerializerOptions options)
//    {
//        writer.WriteStringValue(value.Value.ToDateTime(TimeOnly.MinValue));
//    }
//}

public sealed class ExchangeDateJsonConverter : JsonConverter<ExchangeDate>
{
    private const string Format = "yyyy-MM-dd";

    public override ExchangeDate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (DateOnly.TryParse(dateString, out var date))
        {
            return new ExchangeDate(date);
        }

        throw new JsonException($"Invalid ExchangeDate format: {dateString}");
    }

    public override void Write(Utf8JsonWriter writer, ExchangeDate value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString(Format));
    }

    public override ExchangeDate ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var dateString = reader.GetString();

        if (DateOnly.TryParse(dateString, out var date))
        {
            return new ExchangeDate(date);
        }

        throw new JsonException($"Invalid ExchangeDate key format: {dateString}");
    }

    public override void WriteAsPropertyName(Utf8JsonWriter writer, ExchangeDate value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(value.Value.ToString(Format));
    }
}
