using Ardalis.GuardClauses;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;
using Practice.Backend.CurrencyConverter.Domain.Types.Validators;

namespace Practice.Backend.CurrencyConverter.Domain.Types;

public sealed record ExchangeDate
{
    public DateOnly Value { get; init; }

    public ExchangeDate(DateOnly dateUtc)
    {
        Guard.Against.InvalidInput(dateUtc, nameof(dateUtc), s => !s.IsFutureUtcDate(),
            exceptionCreator: () => new InvalidExchangeDateException("Date cannot be in the future.", nameof(dateUtc)));

        Guard.Against.InvalidInput(dateUtc, nameof(dateUtc), s => s.IsValidDate(),
            exceptionCreator: () => new InvalidExchangeDateException("Date is invalid.", nameof(dateUtc)));

        Value = dateUtc;
    }

    public static ExchangeDate Create(DateOnly dateUtc) => new(dateUtc);

    public static implicit operator DateOnly(ExchangeDate date)
        => date.Value;

    public static implicit operator ExchangeDate(DateOnly dateUtc)
        => new(dateUtc: dateUtc);
}