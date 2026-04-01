using System.Text.Json;
using System.Text.Json.Serialization;
using Practice.Chatbot.CurrencyConverter.Domain.Chat;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Json;

internal sealed class MessageRoleJsonConverter : JsonConverter<MessageRole>
{
    public override MessageRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var name = reader.GetString() ?? throw new JsonException("MessageRole value cannot be null.");
        return MessageRole.FromName(name);
    }

    public override void Write(Utf8JsonWriter writer, MessageRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Name);
    }
}
