using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;

namespace Practice.Backend.CurrencyConverter.Client.Auth;

internal sealed class AuthorizationDelegatingHandler(IServiceProvider serviceProvider)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var tokenProvider = serviceProvider.GetRequiredService<ITokenProvider>();
        var token = await tokenProvider.GetTokenAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
