using System.Net;
using Practice.Backend.CurrencyConverter.Client.Exceptions;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Exceptions;

public sealed class CurrencyConverterApiExceptionSpecifications
{
    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_ReturnsCurrencyConverterApiException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("not found body")
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com/latest", TestContext.Current.CancellationToken);

        exception.Should().BeOfType<CurrencyConverterApiException>();
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_SetsCorrectStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("not found")
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com/latest", TestContext.Current.CancellationToken);

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_SetsResponseContent()
    {
        var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("bad request body")
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com/latest", TestContext.Current.CancellationToken);

        exception.ResponseContent.Should().Be("bad request body");
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_MessageContainsStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com/latest", TestContext.Current.CancellationToken);

        exception.Message.Should().Contain("ServiceUnavailable");
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_MessageContainsRequestUri()
    {
        const string requestUri = "https://api.example.com/api/v1/exchange-rate/latest?BaseCurrency=EUR";
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, requestUri, TestContext.Current.CancellationToken);

        exception.Message.Should().Contain(requestUri);
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_SetsRequestUri()
    {
        const string requestUri = "https://api.example.com/api/v1/exchange-rate/latest";
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("error")
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, requestUri, TestContext.Current.CancellationToken);

        exception.RequestUri.Should().Be(requestUri);
    }

    [Fact]
    public async Task FromResponseAsync_WithFailedResponse_InheritsFromException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com", TestContext.Current.CancellationToken);

        exception.Should().BeAssignableTo<Exception>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public async Task FromResponseAsync_WithVariousStatusCodes_SetsCorrectStatusCode(HttpStatusCode statusCode)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await CurrencyConverterApiException.FromResponseAsync(
            response, "https://api.example.com", TestContext.Current.CancellationToken);

        exception.StatusCode.Should().Be(statusCode);
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ThrowsCurrencyConverterApiException()
    {
        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.OK, "https://api.example.com");

        act.Should().ThrowExactly<CurrencyConverterApiException>();
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ExceptionMessageMentionsEmptyOrUndeserializable()
    {
        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.OK, "https://api.example.com");

        act.Should().ThrowExactly<CurrencyConverterApiException>()
            .Which.Message.Should().ContainAny("empty", "undeserializable");
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ExceptionContainsStatusCode()
    {
        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.NoContent, "https://api.example.com");

        act.Should().ThrowExactly<CurrencyConverterApiException>()
            .Which.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ExceptionContainsRequestUri()
    {
        const string requestUri = "https://api.example.com/api/v1/exchange-rate/latest";

        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.OK, requestUri);

        act.Should().ThrowExactly<CurrencyConverterApiException>()
            .Which.RequestUri.Should().Be(requestUri);
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ResponseContentIsNull()
    {
        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.OK, "https://api.example.com");

        act.Should().ThrowExactly<CurrencyConverterApiException>()
            .Which.ResponseContent.Should().BeNull();
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNotNull_DoesNotThrow()
    {
        var act = () => CurrencyConverterApiException.ThrowIfNull(
            "valid payload", HttpStatusCode.OK, "https://api.example.com");

        act.Should().NotThrow();
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_MessageContainsRequestUri()
    {
        const string requestUri = "https://api.example.com/api/v1/exchange-rate/conversion";

        var act = () => CurrencyConverterApiException.ThrowIfNull<string>(
            null!, HttpStatusCode.OK, requestUri);

        act.Should().ThrowExactly<CurrencyConverterApiException>()
            .Which.Message.Should().Contain(requestUri);
    }
}
