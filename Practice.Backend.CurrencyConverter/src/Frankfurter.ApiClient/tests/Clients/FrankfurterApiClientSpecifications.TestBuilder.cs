using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Clients;

public partial class FrankfurterApiClientSpecifications
{
    private class TestBuilder
    {
        private readonly Mock<ILogger<FrankfurterApiClient>> _loggerMock = new();
        private Func<HttpResponseMessage> _responseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        public string? LastRequestPathAndQuery { get; private set; }

        public TestBuilder WithSuccessResponse(string jsonContent)
        {
            _responseFactory = () => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            return this;
        }

        public TestBuilder WithErrorResponse(HttpStatusCode statusCode, string content)
        {
            _responseFactory = () => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content)
            };

            return this;
        }

        public FrankfurterApiClient Build()
        {
            var handler = new FakeHttpMessageHandler(_responseFactory, uri => LastRequestPathAndQuery = uri);

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.frankfurter.dev/v1/")
            };

            return new FrankfurterApiClient(httpClient, _loggerMock.Object);
        }

        private sealed class FakeHttpMessageHandler(
            Func<HttpResponseMessage> responseFactory,
            Action<string> captureUri)
            : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                captureUri(request.RequestUri?.PathAndQuery ?? string.Empty);

                return Task.FromResult(responseFactory());
            }
        }
    }
}
