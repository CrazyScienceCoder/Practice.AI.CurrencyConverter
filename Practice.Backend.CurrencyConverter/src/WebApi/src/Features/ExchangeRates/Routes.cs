namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates;

public static class Routes
{
    public const string ExchangeRatesPath = "api/v{version:apiVersion}/exchange-rate";

    public static class Endpoints
    {
        public const string Conversion = "conversion";

        public const string Historical = "historical";

        public const string Latest = "latest";
    }

    public static class Version
    {
        public const string V1 = "1.0";
    }
}
