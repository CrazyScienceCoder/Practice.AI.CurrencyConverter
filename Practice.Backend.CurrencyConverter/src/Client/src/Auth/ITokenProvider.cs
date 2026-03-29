namespace Practice.Backend.CurrencyConverter.Client.Auth;

public interface ITokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}
