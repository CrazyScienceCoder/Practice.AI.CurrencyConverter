using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Practice.Chatbot.CurrencyConverter.WebApi.Instrumentation.Authentication;

public static class AuthenticationConfigurator
{
    public static void AddKeycloakJwtAuth(this WebApplicationBuilder builder)
    {
        var keycloakSection = builder.Configuration.GetSection("Keycloak");
        var authority = keycloakSection["Authority"]
            ?? throw new InvalidOperationException("Keycloak:Authority is not configured.");
        var audience = keycloakSection["Audience"]
            ?? throw new InvalidOperationException("Keycloak:Audience is not configured.");
        var requireHttps = bool.Parse(keycloakSection["RequireHttpsMetadata"] ?? "true");
        // Optional: when Keycloak's public URL (browser-facing) differs from its internal
        // Docker hostname, tokens carry the public URL as 'iss'. Set ValidIssuer to that
        // public URL so validation succeeds even though metadata is fetched internally.
        var validIssuer = keycloakSection["ValidIssuer"];

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = requireHttps;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer ?? authority,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AiChat, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireRole("ai:chat"));
        });
    }
}

public static class Policies
{
    public const string AiChat = "ai:chat";
}
