using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using Practice.Backend.CurrencyConverter.Integration.Tests.Infrastructure;

namespace Practice.Backend.CurrencyConverter.Integration.Tests.ExchangeRates.Conversion;

public partial class ConversionIntegrationSpecifications
{
    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_Returns200()
    {
        const string baseCurrency = "JPY";
        const string toCurrency = "USD";
        SetupSuccessResponse(baseCurrency, toCurrency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency={baseCurrency}&toCurrency={toCurrency}&amount=100",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(baseCurrency);
        json["amount"]!.GetValue<decimal>().Should().Be(100m);
        json["date"]!.GetValue<string>().Should().Be("2025-01-15");
        json["rates"]!["USD"]!.GetValue<decimal>().Should().Be(109m);
    }

    [Fact]
    public async Task InvokeAsync_WithValidAuthenticatedRequest_ResponseBodyContainsRates()
    {
        const string baseCurrency = "NOK";
        const string toCurrency = "USD";
        SetupSuccessResponse(baseCurrency, toCurrency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency={baseCurrency}&toCurrency={toCurrency}&amount=50",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        json["base"]!.GetValue<string>().Should().Be(baseCurrency);
        json["amount"]!.GetValue<decimal>().Should().Be(50m);
        json["date"]!.GetValue<string>().Should().Be("2025-01-15");
        json["rates"]!["USD"]!.GetValue<decimal>().Should().Be(54.5m);
    }

    [Fact]
    public async Task InvokeAsync_WithoutAuthorizationHeader_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidBearerToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "this-token-is-invalid");

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvokeAsync_WithTokenMissingRequiredRole_Returns403()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", JwtTokenHelper.GenerateToken("wrong:role"));

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenBaseCurrencyTHB_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=THB&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenToCurrencyTRY_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=TRY&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenBaseCurrencyMXN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=MXN&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithForbiddenToCurrencyPLN_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=PLN&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithAmountLessThanOne_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=USD&amount=0",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidBaseCurrencyCode_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=INVALID&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidToCurrencyCode_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=INVALID&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyBaseCurrency_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WithInvalidProvider_Returns400()
    {
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency=EUR&toCurrency=USD&amount=100&provider=BadProvider",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenProviderReturnsNotFound_Returns404()
    {
        const string baseCurrency = "CAD";
        SetupNotFoundResponse(baseCurrency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency={baseCurrency}&toCurrency=USD&amount=100",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task InvokeAsync_WithNullProvider_DefaultsToFrankfurterAndReturns200()
    {
        const string baseCurrency = "SEK";
        const string toCurrency = "USD";
        SetupSuccessResponse(baseCurrency, toCurrency);
        AuthorizeClient();

        var response = await _client.GetAsync(
            $"{ConversionUrl}?baseCurrency={baseCurrency}&toCurrency={toCurrency}&amount=200",
            TestContext.Current.CancellationToken);
        var body = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var json = JsonNode.Parse(body)!;

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json["base"]!.GetValue<string>().Should().Be(baseCurrency);
        json["amount"]!.GetValue<decimal>().Should().Be(200m);
        json["rates"]!["USD"]!.GetValue<decimal>().Should().Be(218m);
    }
}
