# Practice.Backend.CurrencyConverter.Messages

Shared request and response message contracts for the **Currency Converter API**.

This package is a dependency of `Practice.Backend.CurrencyConverter.Client` and is installed automatically. You only need to reference it directly if you work with the message types independently.

## Installation

```bash
dotnet add package Practice.Backend.CurrencyConverter.Messages
```

## Contracts

### Requests

| Type | Description |
|------|-------------|
| `LatestExchangeRatesRequest` | Fetch latest exchange rates for a base currency |
| `HistoricalExchangeRateRequest` | Fetch historical rates with optional date range and pagination |
| `ConversionRequest` | Convert an amount between two currencies |

### Responses

| Type | Description |
|------|-------------|
| `ExchangeRateResponse` | Base/quote currency, date, and rate dictionary |
| `HistoricalExchangeRateResponse` | Paginated historical rates |
| `ConversionRequest` | Conversion result with converted amount |
