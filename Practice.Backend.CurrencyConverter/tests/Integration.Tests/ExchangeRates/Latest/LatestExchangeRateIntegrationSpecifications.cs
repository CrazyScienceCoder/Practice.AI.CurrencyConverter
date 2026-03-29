using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.ExchangeRates.Latest;

public partial class LatestExchangeRateIntegrationSpecifications
{
    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_Returns200()
    {
        const string currency = "USD";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency={currency}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(currency);
        json["amount"]!.GetValue<decimal>().Should().Be(1m);
        json["date"]!.GetValue<string>().Should().Be("2025-01-15");
        json["rates"]!["EUR"]!.GetValue<decimal>().Should().Be(0.92m);
        json["rates"]!["GBP"]!.GetValue<decimal>().Should().Be(0.79m);
    }

    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_ResponseBodyContainsRates()
    {
        const string currency = "NZD";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency={currency}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        json["base"]!.GetValue<string>().Should().Be(currency);
        json["amount"]!.GetValue<decimal>().Should().Be(1m);
        json["date"]!.GetValue<string>().Should().Be("2025-01-15");
        json["rates"]!["EUR"]!.GetValue<decimal>().Should().Be(0.92m);
        json["rates"]!["GBP"]!.GetValue<decimal>().Should().Be(0.79m);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=USD",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "this-is-not-a-valid-token");

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=USD",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithTokenMissingRequiredRole_Returns403()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken("wrong:role"));

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=USD",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyMXN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=MXN",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidCurrencyCode_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=INVALID",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyBaseCurrency_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidProvider_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=USD&provider=UnknownProvider",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithNullProvider_DefaultsToFrankfurterAndReturns200()
    {
        const string currency = "SGD";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency={currency}",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(currency);
        json["rates"]!["EUR"]!.GetValue<decimal>().Should().Be(0.92m);
        json["rates"]!["GBP"]!.GetValue<decimal>().Should().Be(0.79m);
    }

    [Fact]
    public async Task InvokeAsync_WithExplicitFrankfurterProvider_Returns200()
    {
        const string currency = "HKD";
        SetupSuccessResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency={currency}&provider=Frankfurter",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(currency);
        json["rates"]!["EUR"]!.GetValue<decimal>().Should().Be(0.92m);
        json["rates"]!["GBP"]!.GetValue<decimal>().Should().Be(0.79m);
    }

    [Fact]
    public async Task InvokeAsync_WhenProviderReturnsNotFound_Returns404()
    {
        const string currency = "AUD";
        SetupNotFoundResponse(currency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency={currency}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyPLN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=PLN",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyTHB_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=THB",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenCurrencyTRY_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{LatestUrl}?baseCurrency=TRY",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
