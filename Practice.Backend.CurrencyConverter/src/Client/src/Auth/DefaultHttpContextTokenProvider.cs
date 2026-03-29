using Microsoft.AspNetCore.Http;

namespace Practice.Backend.CurrencyConverter.Client.Auth;

internal sealed class DefaultHttpContextTokenProvider(IHttpContextAccessor httpContextAccessor) : ITokenProvider
{
    public Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        var authHeader = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

        string? token = null;

        if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = authHeader["Bearer ".Length..].Trim();
        }

        return Task.FromResult(token);
    }
}
