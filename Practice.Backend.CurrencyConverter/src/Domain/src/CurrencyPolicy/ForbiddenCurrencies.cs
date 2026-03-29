using Ardalis.SmartEnum;
// ReSharper disable InconsistentNaming

namespace Practice.Backend.CurrencyConverter.Domain.CurrencyPolicy;

public sealed class ForbiddenCurrencies : SmartEnum<ForbiddenCurrencies>
{
    public string Code { get; }

    public string Description { get; }

    private ForbiddenCurrencies(string code, int id, string description)
        : base(name: code, value: id)
    {
        Code = code;
        Description = description;
    }

    public static readonly ForbiddenCurrencies MXN = new(code: nameof(MXN), id: 1, description: "Mexican Peso");
    public static readonly ForbiddenCurrencies PLN = new(code: nameof(PLN), id: 2, description: "Polish Zloty");
    public static readonly ForbiddenCurrencies THB = new(code: nameof(THB), id: 3, description: "Thai Baht");
    public static readonly ForbiddenCurrencies TRY = new(code: nameof(TRY), id: 4, description: "Turkish Lira");
}