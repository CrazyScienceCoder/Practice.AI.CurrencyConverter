namespace Practice.Backend.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidCurrencyCodeException(string message, string parameterName) : DomainValidationException(message, parameterName);