namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

public sealed class JwtAuthOptions
{
    public const string SectionName = "JwtAuth";

    public required string Authority { get; init; }
    public required string Audience { get; init; }
    public string? ValidIssuer { get; init; }
    public bool RequireHttpsMetadata { get; init; } = true;
    public bool ValidateIssuer { get; init; } = true;
    public bool ValidateAudience { get; init; } = true;
    public bool ValidateLifetime { get; init; } = true;
    public bool ValidateIssuerSigningKey { get; init; } = true;
    public int ClockSkewSeconds { get; init; } = 0;
}
