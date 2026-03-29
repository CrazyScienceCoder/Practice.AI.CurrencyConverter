namespace Practice.Backend.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidExchangeDateException(string message, string parameterName) : DomainValidationException(message, parameterName);