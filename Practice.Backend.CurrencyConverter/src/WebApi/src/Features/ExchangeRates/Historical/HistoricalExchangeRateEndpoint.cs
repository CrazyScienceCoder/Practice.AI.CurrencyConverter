using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;
using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates.Historical;
using Practice.Backend.CurrencyConverter.WebApi.ActionResultBuilders.Factories;
using Practice.Backend.CurrencyConverter.WebApi.Instrumentation.Authentication;

namespace Practice.Backend.CurrencyConverter.WebApi.Features.ExchangeRates.Historical;

[Route(Routes.ExchangeRatesPath)]
[ApiVersion(Routes.Version.V1)]
[ApiController]
[Authorize(Policy = Policies.CurrencyAdmin)]
public sealed class HistoricalExchangeRateEndpoint(IMediator mediator, IActionResultBuilderFactory factory)
    : WebApiBaseController(factory)
{
    [HttpGet(template: Routes.Endpoints.Historical)]
    [ProducesResponseType(type: typeof(GetHistoricalExchangeRateQueryResult), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest)]
    [ProducesResponseType(statusCode: StatusCodes.Status404NotFound)]
    [ProducesResponseType(statusCode: StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> InvokeAsync([FromQuery] HistoricalExchangeRateRequest request
        , CancellationToken cancellationToken)
    {
        var query = request.ToQuery();

        var result = await mediator.Send(query, cancellationToken);

        return BuildResponse(result);
    }
}
