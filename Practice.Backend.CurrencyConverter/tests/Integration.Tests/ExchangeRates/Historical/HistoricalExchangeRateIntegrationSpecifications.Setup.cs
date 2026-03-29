using System.Net.Http.Headers;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;
using Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.ExchangeRates.Historical;

public partial class HistoricalExchangeRateIntegrationSpecifications(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private const string HistoricalUrl = "/api/v1/exchange-rate/historical";
    private const string CurrencyAdminRole = "currency:admin";

    // Stable past dates that are weekdays (Mon-Fri) and satisfy ExchangeDate validation
    private const string FromDate = "2025-01-06";
    private const string ToDate = "2025-01-10";

    private readonly HttpClient _client = factory.CreateClient();

    private void AuthorizeClient()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(CurrencyAdminRole));
    }

    private void SetupSuccessResponse(string baseCurrency)
    {
        factory.FrankfurterClientMock
            .Setup(c => c.GetTimeSeriesAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                baseCurrency,
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TimeSeriesResponse
            {
                Amount = 1,
                Base = baseCurrency,
                StartDate = new DateTime(2025, 1, 6),
                EndDate = new DateTime(2025, 1, 10),
                Rates = new Dictionary<string, Dictionary<string, double>>
                {
                    ["2025-01-06"] = new() { ["USD"] = 1.09 },
                    ["2025-01-07"] = new() { ["USD"] = 1.10 },
                    ["2025-01-08"] = new() { ["USD"] = 1.08 },
                    ["2025-01-09"] = new() { ["USD"] = 1.11 },
                    ["2025-01-10"] = new() { ["USD"] = 1.07 }
                }
            });
    }

    private void SetupNotFoundResponse(string baseCurrency)
    {
        factory.FrankfurterClientMock
            .Setup(c => c.GetTimeSeriesAsync(
                It.IsAny<DateOnly>(),
                It.IsAny<DateOnly>(),
                baseCurrency,
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FrankfurterApiException(
                "Not found",
                System.Net.HttpStatusCode.NotFound));
    }
}
