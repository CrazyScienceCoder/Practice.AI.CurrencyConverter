using System.Globalization;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

internal static class FrankfurterEndpoints
{
    public const string Latest = "latest";

    public static string ForDate(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    public static string ForRange(DateOnly from, DateOnly to) => $"{from:yyyy-MM-dd}..{to:yyyy-MM-dd}";
}