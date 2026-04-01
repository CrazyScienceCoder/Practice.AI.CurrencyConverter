using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Conversion;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;

namespace Practice.Backend.CurrencyConverter.Client.Internal;

internal static class QueryBuilder
{
    private const string DateFormat = "yyyy-MM-dd";

    internal static Dictionary<string, string?> ForLatest(LatestExchangeRatesRequest request)
    {
        var query = new Dictionary<string, string?>
        {
            ["BaseCurrency"] = request.BaseCurrency
        };

        AddProvider(query, request.Provider);

        return query;
    }

    internal static Dictionary<string, string?> ForHistorical(HistoricalExchangeRateRequest request)
    {
        var query = new Dictionary<string, string?>
        {
            ["BaseCurrency"] = request.BaseCurrency
        };

        if (request.From.HasValue)
        {
            query["From"] = request.From.Value.ToString(DateFormat);
        }

        if (request.To.HasValue)
        {
            query["To"] = request.To.Value.ToString(DateFormat);
        }

        if (request.PageNumber.HasValue)
        {
            query["PageNumber"] = request.PageNumber.Value.ToString();
        }

        if (request.DaysPerPage.HasValue)
        {
            query["DaysPerPage"] = request.DaysPerPage.Value.ToString();
        }

        AddProvider(query, request.Provider);

        return query;
    }

    internal static Dictionary<string, string?> ForConversion(ConversionRequest request)
    {
        var query = new Dictionary<string, string?>
        {
            ["BaseCurrency"] = request.BaseCurrency,
            ["ToCurrency"] = request.ToCurrency,
            ["Amount"] = request.Amount.ToString("G")
        };

        AddProvider(query, request.Provider);

        return query;
    }

    private static void AddProvider(Dictionary<string, string?> query, string? provider)
    {
        if (!string.IsNullOrWhiteSpace(provider))
        {
            query["Provider"] = provider;
        }
    }
}
