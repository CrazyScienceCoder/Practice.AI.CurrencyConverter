using System.Net;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Extensions;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Extensions;

public sealed class HttpResponseMessageExtensionsSpecifications
{
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Created)]
    [InlineData(HttpStatusCode.Accepted)]
    [InlineData(HttpStatusCode.NoContent)]
    public async Task EnsureSuccessAsync_ResponseIsSuccessful_DoesNotThrow(HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(string.Empty)
        };

        var act = () => response.EnsureSuccessAsync("https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        await act.Should().NotThrowAsync();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task EnsureSuccessAsync_ResponseIsNotSuccessful_ThrowsFrankfurterApiException(HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent("error response")
        };

        var act = () => response.EnsureSuccessAsync("https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        await act.Should().ThrowExactlyAsync<FrankfurterApiException>();
    }

    [Fact]
    public async Task EnsureSuccessAsync_ResponseIsNotSuccessful_ExceptionContainsStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("not found")
        };

        var act = () => response.EnsureSuccessAsync("https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task EnsureSuccessAsync_ResponseIsNotSuccessful_ExceptionContainsResponseContent()
    {
        const string errorBody = "rate limit exceeded";
        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(errorBody)
        };

        var act = () => response.EnsureSuccessAsync("https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        (await act.Should().ThrowExactlyAsync<FrankfurterApiException>())
            .Which.ResponseContent.Should().Be(errorBody);
    }
}
