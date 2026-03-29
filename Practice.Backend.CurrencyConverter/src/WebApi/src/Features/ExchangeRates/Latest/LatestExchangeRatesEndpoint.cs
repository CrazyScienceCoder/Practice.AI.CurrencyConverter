using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetLatest;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Latest;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Latest;

[Route(Routes.ExchangeRatesPath)]
[ApiVersion(Routes.Version.V1)]
[ApiController]
[Authorize(Policy = Policies.CurrencyRead)]
public sealed class LatestExchangeRatesEndpoint(IMediator mediator, IActionResultBuilderFactory factory)
    : WebApiBaseController(factory)
{
    [HttpGet(template: Routes.Endpoints.Latest)]
    [ProducesResponseType(type: typeof(GetLatestExchangeRateQueryResult), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InvokeAsync([FromQuery] LatestExchangeRatesRequest request
        , CancellationToken cancellationToken)
    {
        var query = request.ToQuery();

        var result = await mediator.Send(query, cancellationToken);

        return BuildResponse(result);
    }
}
