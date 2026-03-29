using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Practice.Backend.CurrencyConverter.Client.Auth;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Auth;

public sealed class DefaultHttpContextTokenProviderSpecifications
{
    [Fact]
    public async Task GetTokenAsync_WithNullHttpContext_ReturnsNull()
    {
        var accessorMock = new Mock<IHttpContextAccessor>();
        var sut = new DefaultHttpContextTokenProvider(accessorMock.Object);

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTokenAsync_WithBearerToken_ReturnsToken()
    {
        var sut = BuildProviderWithAuthHeader("Bearer my-jwt-token");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().Be("my-jwt-token");
    }

    [Fact]
    public async Task GetTokenAsync_WithBearerTokenPaddedWithSpaces_ReturnsTrimmedToken()
    {
        var sut = BuildProviderWithAuthHeader("Bearer   my-token  ");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().Be("my-token");
    }

    [Fact]
    public async Task GetTokenAsync_WithBearerPrefixInLowerCase_ReturnsToken()
    {
        var sut = BuildProviderWithAuthHeader("bearer my-jwt-token");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().Be("my-jwt-token");
    }

    [Fact]
    public async Task GetTokenAsync_WithBearerPrefixInUpperCase_ReturnsToken()
    {
        var sut = BuildProviderWithAuthHeader("BEARER my-jwt-token");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().Be("my-jwt-token");
    }

    [Fact]
    public async Task GetTokenAsync_WithNonBearerScheme_ReturnsNull()
    {
        var sut = BuildProviderWithAuthHeader("Basic dXNlcjpwYXNz");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTokenAsync_WithEmptyAuthorizationHeader_ReturnsNull()
    {
        var sut = BuildProviderWithAuthHeader(string.Empty);

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTokenAsync_WithWhitespaceAuthorizationHeader_ReturnsNull()
    {
        var sut = BuildProviderWithAuthHeader("   ");

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTokenAsync_WithNoAuthorizationHeader_ReturnsNull()
    {
        var sut = BuildProviderWithAuthHeader(StringValues.Empty);

        var result = await sut.GetTokenAsync(TestContext.Current.CancellationToken);

        result.Should().BeNull();
    }

    private static DefaultHttpContextTokenProvider BuildProviderWithAuthHeader(string headerValue)
        => BuildProviderWithAuthHeader(new StringValues(headerValue));

    private static DefaultHttpContextTokenProvider BuildProviderWithAuthHeader(StringValues headerValue)
    {
        var headersMock = new Mock<IHeaderDictionary>();
        headersMock
            .Setup(h => h["Authorization"])
            .Returns(headerValue);

        var requestMock = new Mock<HttpRequest>();
        requestMock.Setup(r => r.Headers).Returns(headersMock.Object);

        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(c => c.Request).Returns(requestMock.Object);

        var accessorMock = new Mock<IHttpContextAccessor>();
        accessorMock.Setup(a => a.HttpContext).Returns(contextMock.Object);

        return new DefaultHttpContextTokenProvider(accessorMock.Object);
    }
}
