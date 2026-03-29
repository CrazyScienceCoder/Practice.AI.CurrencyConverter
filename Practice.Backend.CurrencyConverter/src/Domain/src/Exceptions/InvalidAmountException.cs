namespace Practice.Backend.CurrencyConverter.Domain.Exceptions;

public sealed class InvalidAmountException(string message, string parameterName) : DomainValidationException(message, parameterName);