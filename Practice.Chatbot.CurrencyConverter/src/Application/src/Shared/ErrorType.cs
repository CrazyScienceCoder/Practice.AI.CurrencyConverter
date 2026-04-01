using Ardalis.SmartEnum;

namespace Practice.Chatbot.CurrencyConverter.Application.Shared;

public sealed class ErrorType : SmartEnum<ErrorType>
{
    public static readonly ErrorType Generic = new(nameof(Generic), 1);
    public static readonly ErrorType NotFound = new(nameof(NotFound), 2);
    public static readonly ErrorType ValidationError = new(nameof(ValidationError), 3);
    public static readonly ErrorType NotAllowed = new(nameof(NotAllowed), 4);

    private ErrorType(string name, int value) : base(name, value)
    {
    }
}
