using System.Globalization;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

internal sealed class QueryBuilder
{
    private readonly Dictionary<string, string?> _query = new(capacity: 3);

    private QueryBuilder(string baseCurrency)
    {
        _query[QueryParameters.Base] = baseCurrency;
    }

    public static QueryBuilder Create(string baseCurrency) => new(baseCurrency);

    public QueryBuilder WithAmount(double amount)
    {
        _query[QueryParameters.Amount] = amount.ToString("0.####", CultureInfo.InvariantCulture);

        return this;
    }

    public QueryBuilder WithSymbols(string[]? symbols)
    {
        if (symbols is { Length: > 0 })
        {
            _query[QueryParameters.Symbols] = string.Join(',', symbols);
        }

        return this;
    }

    public IReadOnlyDictionary<string, string?> Build() => _query;
}