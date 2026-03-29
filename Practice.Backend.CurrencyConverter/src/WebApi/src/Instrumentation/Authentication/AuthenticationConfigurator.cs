using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

public static class AuthenticationConfigurator
{
    public static void AddJwtAuth(this WebApplicationBuilder builder)
    {
        var jwtOptions = builder.Configuration
            .GetSection(JwtAuthOptions.SectionName)
            .Get<JwtAuthOptions>()
            ?? throw new InvalidOperationException($"'{JwtAuthOptions.SectionName}' configuration section is missing.");

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = jwtOptions.Authority;
                options.Audience = jwtOptions.Audience;
                options.RequireHttpsMetadata = jwtOptions.RequireHttpsMetadata;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidIssuer = jwtOptions.ValidIssuer ?? jwtOptions.Authority,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidateLifetime = jwtOptions.ValidateLifetime,
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds)
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx =>
                    {
                        ctx.Response.Headers["WWW-Authenticate-Error"] = ctx.Exception.Message;
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CurrencyRead,
                policy => policy.RequireAuthenticatedUser().RequireRole(Policies.CurrencyRead))
            .AddPolicy(Policies.CurrencyAdmin,
                policy => policy.RequireAuthenticatedUser().RequireRole(Policies.CurrencyAdmin));
    }
}

public static class Policies
{
    public const string CurrencyRead = "currency:read";
    public const string CurrencyAdmin = "currency:admin";
}
