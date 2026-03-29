using Ardalis.GuardClauses;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Types;

public sealed record Currency
{
    public const int MaxLength = 3;

    public string Value { get; init; }

    public Currency(string currencyCode)
    {
        Guard.Against.NullOrEmpty(currencyCode,
            exceptionCreator: () => new InvalidCurrencyCodeException("Currency code cannot be null or empty.", nameof(currencyCode)));

        Guard.Against.StringTooLong(currencyCode, MaxLength,
            exceptionCreator: () => new InvalidCurrencyCodeException($"Currency code max length is {MaxLength} chars.*", nameof(currencyCode)));

        Value = currencyCode;
    }

    public static Currency Create(string currencyCode) => new(currencyCode);

    public static implicit operator Currency(string currencyCode)
        => new(currencyCode);

    public static implicit operator string(Currency currency)
        => currency.Value;
}
