namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Configurations;

public sealed class Redis
{
    public required string ConnectionString { get; set; }

    public required string InstanceName { get; set; }
}
