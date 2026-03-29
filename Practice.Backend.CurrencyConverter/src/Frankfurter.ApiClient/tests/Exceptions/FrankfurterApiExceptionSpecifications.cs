using System.Net;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Exceptions;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Exceptions;

public sealed class FrankfurterApiExceptionSpecifications
{
    [Fact]
    public void Constructor_WithStatusCode_StoresStatusCode()
    {
        var exception = new FrankfurterApiException("Test message", HttpStatusCode.NotFound);

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void Constructor_WithResponseContent_StoresResponseContent()
    {
        var exception = new FrankfurterApiException("Test message", HttpStatusCode.NotFound, "response body");

        exception.ResponseContent.Should().Be("response body");
    }

    [Fact]
    public void Constructor_WithoutResponseContent_ResponseContentIsNull()
    {
        var exception = new FrankfurterApiException("Test message", HttpStatusCode.BadRequest);

        exception.ResponseContent.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithMessage_ExposesMessageViaBaseException()
    {
        var exception = new FrankfurterApiException("Frankfurter failed.", HttpStatusCode.NotFound);

        exception.Message.Should().Be("Frankfurter failed.");
    }

    [Fact]
    public void Constructor_Always_InheritsFromException()
    {
        var exception = new FrankfurterApiException("Test message", HttpStatusCode.NotFound);

        exception.Should().BeAssignableTo<Exception>();
    }

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    public void Constructor_WithVariousStatusCodes_StoresCorrectStatusCode(HttpStatusCode statusCode)
    {
        var exception = new FrankfurterApiException("Test", statusCode);

        exception.StatusCode.Should().Be(statusCode);
    }

    [Fact]
    public async Task FromHttpResponseAsync_WithFailedResponse_ReturnsExceptionWithCorrectStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("not found body")
        };

        var exception = await FrankfurterApiException.FromHttpResponseAsync(
            response, "https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task FromHttpResponseAsync_WithFailedResponse_ReturnsExceptionWithResponseContent()
    {
        var response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("not found body")
        };

        var exception = await FrankfurterApiException.FromHttpResponseAsync(
            response, "https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        exception.ResponseContent.Should().Be("not found body");
    }

    [Fact]
    public async Task FromHttpResponseAsync_WithFailedResponse_MessageContainsStatusCode()
    {
        var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await FrankfurterApiException.FromHttpResponseAsync(
            response, "https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        exception.Message.Should().Contain("ServiceUnavailable");
    }

    [Fact]
    public async Task FromHttpResponseAsync_WithFailedResponse_MessageContainsRequestUri()
    {
        const string requestUri = "https://api.frankfurter.dev/v1/latest";
        var response = new HttpResponseMessage(HttpStatusCode.BadGateway)
        {
            Content = new StringContent(string.Empty)
        };

        var exception = await FrankfurterApiException.FromHttpResponseAsync(
            response, requestUri, TestContext.Current.CancellationToken);

        exception.Message.Should().Contain(requestUri);
    }

    [Fact]
    public async Task FromHttpResponseAsync_WithFailedResponse_ReturnsFrankfurterApiException()
    {
        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("error")
        };

        var exception = await FrankfurterApiException.FromHttpResponseAsync(
            response, "https://api.frankfurter.dev/v1/latest", TestContext.Current.CancellationToken);

        exception.Should().BeOfType<FrankfurterApiException>();
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ThrowsFrankfurterApiException()
    {
        var act = () => FrankfurterApiException.ThrowIfNull<string>(null!, HttpStatusCode.OK);

        act.Should().ThrowExactly<FrankfurterApiException>();
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ExceptionMessageMentionsEmptyPayload()
    {
        var act = () => FrankfurterApiException.ThrowIfNull<string>(null!, HttpStatusCode.OK);

        act.Should().ThrowExactly<FrankfurterApiException>()
            .Which.Message.Should().Contain("empty payload");
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNull_ExceptionContainsStatusCode()
    {
        var act = () => FrankfurterApiException.ThrowIfNull<string>(null!, HttpStatusCode.NoContent);

        act.Should().ThrowExactly<FrankfurterApiException>()
            .Which.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public void ThrowIfNull_PayloadIsNotNull_DoesNotThrow()
    {
        var act = () => FrankfurterApiException.ThrowIfNull("valid payload", HttpStatusCode.OK);

        act.Should().NotThrow();
    }
}
