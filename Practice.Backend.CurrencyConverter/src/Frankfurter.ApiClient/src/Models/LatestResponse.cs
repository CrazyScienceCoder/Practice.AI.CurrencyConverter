using System.Text.Json.Serialization;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;

public class LatestResponse
{
    public double Amount { get; set; }

    public string Base { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public Dictionary<string, double> Rates { get; set; } = new();
}

public sealed class HistoricalResponse : LatestResponse { }

public sealed class TimeSeriesResponse
{
    public double Amount { get; set; }

    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("start_date")]
    public DateTime StartDate { get; set; }

    [JsonPropertyName("end_date")]
    public DateTime EndDate { get; set; }

    public Dictionary<string, Dictionary<string, double>> Rates { get; set; } = new();
}
