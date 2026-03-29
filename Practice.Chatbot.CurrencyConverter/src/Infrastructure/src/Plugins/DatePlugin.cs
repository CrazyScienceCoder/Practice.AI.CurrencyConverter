using System.ComponentModel;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

public sealed class DatePlugin
{
    [Description("Gets today's date in ISO 8601 format (YYYY-MM-DD). Use this when the user asks about the current date or when you need today's date to resolve relative date expressions like 'today' or 'now'.")]
    public static PluginResult<string> GetToday()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow).ToString("yyyy-MM-dd");
        return PluginResult<string>.Success(today);
    }
}