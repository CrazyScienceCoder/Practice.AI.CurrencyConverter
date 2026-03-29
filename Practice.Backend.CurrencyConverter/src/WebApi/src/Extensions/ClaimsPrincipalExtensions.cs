using System.Security.Claims;

namespace Practice.Backend.CurrencyConverter.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal principal)
        => principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("sub")
            ?? "anonymous";
}