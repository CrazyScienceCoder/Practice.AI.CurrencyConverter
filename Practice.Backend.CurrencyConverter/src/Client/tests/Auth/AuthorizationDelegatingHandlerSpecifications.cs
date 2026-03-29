using System.Net;
using Practice.Backend.CurrencyConverter.Client.Auth;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Auth;

public sealed class AuthorizationDelegatingHandlerSpecifications
{
    [Fact]
    public async Task SendAsync_WithValidToken_AddsAuthorizationHeader()
    {
        using var handler = BuildHandler("test-token");
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization.Should().NotBeNull();
    }

    [Fact]
    public async Task SendAsync_WithValidToken_UsesBearerScheme()
    {
        using var handler = BuildHandler("test-token");
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization!.Scheme.Should().Be("Bearer");
    }

    [Fact]
    public async Task SendAsync_WithValidToken_SetsTokenAsParameter()
    {
        using var handler = BuildHandler("my-jwt-token");
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization!.Parameter.Should().Be("my-jwt-token");
    }

    [Fact]
    public async Task SendAsync_WithNullToken_DoesNotAddAuthorizationHeader()
    {
        using var handler = BuildHandler(null);
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_WithWhitespaceToken_DoesNotAddAuthorizationHeader()
    {
        using var handler = BuildHandler("   ");
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_WithEmptyToken_DoesNotAddAuthorizationHeader()
    {
        using var handler = BuildHandler(string.Empty);
        using var invoker = new HttpMessageInvoker(handler);
        var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost/test");

        await invoker.SendAsync(request, TestContext.Current.CancellationToken);

        request.Headers.Authorization.Should().BeNull();
    }

    [Fact]
    public async Task SendAsync_Always_ForwardsRequestToInnerHandler()
    {
        var innerHandlerCalled = false;
        var tokenProviderMock = new Mock<ITokenProvider>();
        tokenProviderMock
            .Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync("token");

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ITokenProvider)))
            .Returns(tokenProviderMock.Object);

        using var inner = new CallbackHttpMessageHandler(_ => innerHandlerCalled = true);
        using var handler = new AuthorizationDelegatingHandler(serviceProviderMock.Object)
        {
            InnerHandler = inner
        };
        using var invoker = new HttpMessageInvoker(handler);

        await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "http://localhost/test"),
            TestContext.Current.CancellationToken);

        innerHandlerCalled.Should().BeTrue();
    }

    [Fact]
    public async Task SendAsync_Always_ReturnsResponseFromInnerHandler()
    {
        using var handler = BuildHandler("token");
        using var invoker = new HttpMessageInvoker(handler);

        var response = await invoker.SendAsync(
            new HttpRequestMessage(HttpMethod.Get, "http://localhost/test"),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static AuthorizationDelegatingHandler BuildHandler(string? token)
    {
        var tokenProviderMock = new Mock<ITokenProvider>();
        tokenProviderMock
            .Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(sp => sp.GetService(typeof(ITokenProvider)))
            .Returns(tokenProviderMock.Object);

        return new AuthorizationDelegatingHandler(serviceProviderMock.Object)
        {
            InnerHandler = new NoOpHttpMessageHandler()
        };
    }

    private sealed class NoOpHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
    }

    private sealed class CallbackHttpMessageHandler(Action<HttpRequestMessage> onSend) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            onSend(request);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }
}
