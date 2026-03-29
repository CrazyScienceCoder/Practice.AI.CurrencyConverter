namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

public sealed record PluginResult<T>
{
    public T? Data { get; init; }

    public string? Error { get; init; }

    public bool IsSuccess => Error is null;

    public static PluginResult<T> Success(T data) => new() { Data = data };

    public static PluginResult<T> Failure(string error) => new() { Error = error };
}
