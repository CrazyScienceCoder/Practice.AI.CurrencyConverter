# Practice.Backend.CurrencyConverter.Client

A typed, DI-friendly HTTP client library for the **Currency Converter API**.
Publish as a NuGet package to share across services or environments.

## Features

- Typed `ICurrencyConverterClient` interface — easy to mock in tests
- Configurable base URL — supports local, staging, and production environments
- JWT bearer authentication via pluggable `ITokenProvider`
- Built-in resilience: exponential retry + circuit breaker (via `Microsoft.Extensions.Http.Resilience`)
- Options validation on startup — misconfiguration fails fast
- Fully compatible with `IHttpClientFactory` and Microsoft's generic host

## Installation

```bash
dotnet add package Practice.Backend.CurrencyConverter.Client
```

## Quick Start

### 1. Configure via `appsettings.json`

```json
{
  "CurrencyConverterClient": {
    "BaseUrl": "https://api.example.com",
    "ApiVersion": "1",
    "TimeoutSeconds": 30,
    "Retry": {
      "MaxAttempts": 3,
      "InitialDelaySeconds": 2
    },
    "CircuitBreaker": {
      "FailureRatio": 0.5,
      "BreakDurationSeconds": 60,
      "SamplingDurationSeconds": 60,
      "MinimumThroughput": 30
    }
  }
}
```

### 2. Register in DI

```csharp
// From configuration section (recommended)
services.AddCurrencyConverterClient(
    builder.Configuration.GetSection(CurrencyConverterClientOptions.SectionName));

// Or inline
services.AddCurrencyConverterClient(options =>
{
    options.BaseUrl = "http://localhost:9081";
    options.TimeoutSeconds = 15;
});
```

### 3. Add Authentication (optional)

**Option A — forward the incoming bearer token (ASP.NET Core apps):**

```csharp
services.AddDefaultHttpContextTokenProvider();
```

This forwards the `Authorization: Bearer ...` header from the current HTTP context — useful when the calling service already has a Keycloak token.

**Option B — custom provider:**

```csharp
public class KeycloakTokenProvider(IKeycloakClient keycloak) : ITokenProvider
{
    public async Task<string?> GetTokenAsync(CancellationToken ct = default)
        => await keycloak.GetAccessTokenAsync(ct);
}

services.AddSingleton<ITokenProvider, KeycloakTokenProvider>();
```

If no `ITokenProvider` is registered, requests are sent without an `Authorization` header.

### 4. Inject and use

```csharp
public class MyService(ICurrencyConverterClient client)
{
    public async Task<decimal> GetEurToUsdRateAsync(CancellationToken ct)
    {
        var response = await client.GetLatestExchangeRatesAsync(
            new LatestExchangeRatesRequest { BaseCurrency = "EUR" },
            ct);

        return response.Rates.GetValueOrDefault("USD");
    }

    public async Task<decimal> ConvertAsync(decimal amount, CancellationToken ct)
    {
        var response = await client.GetConversionAsync(
            new ConversionRequest { BaseCurrency = "EUR", ToCurrency = "USD", Amount = amount },
            ct);

        return response.Rates.GetValueOrDefault("USD");
    }
}
```

## API

### `ICurrencyConverterClient`

| Method | Returns | Description |
|---|---|---|
| `GetLatestExchangeRatesAsync(request, ct)` | `ExchangeRateResponse` | Latest rates for a base currency |
| `GetHistoricalExchangeRatesAsync(request, ct)` | `HistoricalExchangeRateResponse` | Historical rates with optional date range and pagination |
| `GetConversionAsync(request, ct)` | `ExchangeRateResponse` | Convert an amount between two currencies |

All methods throw `CurrencyConverterApiException` on API errors.

### `CurrencyConverterApiException`

| Property | Description |
|---|---|
| `StatusCode` | HTTP status code from the API |
| `RequestUri` | The URI that produced the error |
| `ResponseContent` | Raw response body (for diagnostics) |
