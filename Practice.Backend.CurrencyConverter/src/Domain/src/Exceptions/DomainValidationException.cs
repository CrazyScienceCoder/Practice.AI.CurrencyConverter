namespace Practice.Backend.CurrencyConverter.Domain.Exceptions;

public abstract class DomainValidationException(string message, string parameterName) : ArgumentException(message, parameterName);