namespace Practice.Backend.CurrencyConverter.Domain.Types.Validators;

public static class ExchangeDateValidator
{
    extension(DateOnly dateTime)
    {
        public bool IsFutureUtcDate() => dateTime > DateOnly.FromDateTime(DateTime.UtcNow);

        public bool IsValidDate() => dateTime > DateOnly.MinValue;
    }
}