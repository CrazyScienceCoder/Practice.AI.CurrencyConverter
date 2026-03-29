using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Practice.Backend.CurrencyConverter.Client.Configuration;

namespace Practice.Backend.CurrencyConverter.Client.Tests.Clients;

public partial class CurrencyConverterClientSpecifications
{
    private sealed class TestBuilder
    {
        // NullLogger avoids Castle DynamicProxy issues with internal generic type arguments
        private readonly ILogger<CurrencyConverterClient> _logger =
            NullLogger<CurrencyConverterClient>.Instance;

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

        public CurrencyConverterClient Build()
        {
            var handler = new FakeHttpMessageHandler(
                _responseFactory,
                uri => LastRequestPathAndQuery = uri);

            var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://api.example.com/")
            };

            var options = Options.Create(new CurrencyConverterClientOptions
            {
                BaseUrl = "http://api.example.com",
                ApiVersion = "1"
            });

            return new CurrencyConverterClient(httpClient, options, _logger);
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
