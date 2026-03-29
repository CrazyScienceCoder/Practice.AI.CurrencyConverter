using System.Net.Http.Headers;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Models;
using Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.ExchangeRates.Latest;

public partial class LatestExchangeRateIntegrationSpecifications(CustomWebApplicationFactory factory)
    : IClassFixture<CustomWebApplicationFactory>
{
    private const string LatestUrl = "/api/v1/exchange-rate/latest";
    private const string CurrencyReadRole = "currency:read";

    private readonly HttpClient _client = factory.CreateClient();

    private void AuthorizeClient()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken(CurrencyReadRole));
    }

    private void SetupSuccessResponse(string baseCurrency)
    {
        factory.FrankfurterClientMock
            .Setup(c => c.GetLatestAsync(
                baseCurrency,
                It.IsAny<double>(),
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LatestResponse
            {
                Amount = 1,
                Base = baseCurrency,
                Date = new DateTime(2025, 1, 15),
                Rates = new Dictionary<string, double> { ["EUR"] = 0.92, ["GBP"] = 0.79 }
            });
    }

    private void SetupNotFoundResponse(string baseCurrency)
    {
        factory.FrankfurterClientMock
            .Setup(c => c.GetLatestAsync(
                baseCurrency,
                It.IsAny<double>(),
                It.IsAny<string[]?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FrankfurterApiException(
                "Not found",
                System.Net.HttpStatusCode.NotFound));
    }
}
