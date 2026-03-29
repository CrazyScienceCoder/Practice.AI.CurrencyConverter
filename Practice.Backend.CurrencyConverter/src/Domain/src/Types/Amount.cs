using Ardalis.GuardClauses;
using Practice.Backend.CurrencyConverter.Domain.Exceptions;

namespace Practice.Backend.CurrencyConverter.Domain.Types;

public sealed record Amount
{
    public decimal Value { get; init; }

    public Amount(decimal amount)
    {
        Guard.Against.NegativeOrZero(amount,
            exceptionCreator: () => new InvalidAmountException("Amount cannot be negative or Zero.", nameof(amount)));

        Value = amount;
    }

    public static Amount Create(decimal amount) => new(amount);

    public static implicit operator decimal(Amount amount)
        => amount.Value;

    public static implicit operator Amount(decimal amount)
        => new(amount);
}
