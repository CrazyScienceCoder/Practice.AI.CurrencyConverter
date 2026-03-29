using ErrorOr;
using Microsoft.Extensions.Logging;
using Practice.Backend.CurrencyConverter.Application.Shared;
using Practice.Backend.CurrencyConverter.Application.Shared.Mappers;
using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;
using ErrorType = Practice.Backend.CurrencyConverter.Application.Shared.ErrorType;

namespace Practice.Backend.CurrencyConverter.Application.ExchangeRates.GetHistorical;

public sealed class GetHistoricalExchangeRateQueryHandler(
    IExchangeRateProviderFactory providerFactory,
    ILogger<GetHistoricalExchangeRateQueryHandler> logger)
    : HandlerBase<GetHistoricalExchangeRateQuery, GetHistoricalExchangeRateQueryResponse,
        GetHistoricalExchangeRateQueryResult>(logger)
{
    private const int DefaultPageNumber = 1;
    private const int DefaultDaysPerPage = 10;

    protected override async Task<GetHistoricalExchangeRateQueryResponse> ExecuteAsync(GetHistoricalExchangeRateQuery request
        , CancellationToken cancellationToken)
    {
        var provider = providerFactory.Create(request.Provider!);

        var pageNumber = request.PageNumber.GetValueOrDefault(DefaultPageNumber);

        var daysPerPage = request.DaysPerPage.GetValueOrDefault(DefaultDaysPerPage);

        var businessDays = TradingCalendar.GetBusinessDays(request.From, request.To);

        var totalNumberOfPages = CalculateTotalNumberOfPages(businessDays.Count, daysPerPage);

        var pageDates = GetPageDates(pageNumber, daysPerPage, businessDays);

        if (pageDates.Count == 0)
        {
            return GetHistoricalExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound
                , message: $"No valid business days found within the specified range. BaseCurrency: {request.BaseCurrency}, From: {request.From}, To: {request.To}");
        }

        var fromDate = pageDates.First();

        var toDate = pageDates.Last();

        var hasMore = toDate < businessDays.Last();

        var result = await provider.GetHistoricalExchangeRateAsync(baseCurrency: request.BaseCurrency
            , from: fromDate
            , to: toDate
            , cancellationToken: cancellationToken);

        return result.Match(
            onValue: exchangeRate => GetHistoricalExchangeRateQueryResponse.Success(
                data: exchangeRate.FilterExcludedCurrencies()
                    .ToHistoricalExchangeRateResult(pageNumber, totalNumberOfPages, hasMore)
                , message: "Historical exchange rate was retrieved successfully"),
            onError: errors => HandleError(request, errors.First()));
    }

    private static GetHistoricalExchangeRateQueryResponse HandleError(GetHistoricalExchangeRateQuery request
        , Error error)
    {
        return error.Type is ErrorOr.ErrorType.NotFound
            ? GetHistoricalExchangeRateQueryResponse.Failure(errorType: ErrorType.NotFound
                , message: $"Historical exchange rate was not found, BaseCurrency: {request.BaseCurrency}, From: {request.From}, To: {request.To}")
            : GetHistoricalExchangeRateQueryResponse.Failure(errorType: ErrorType.Generic
                , message: $"{error.Description}, BaseCurrency: {request.BaseCurrency}, From: {request.From}, To: {request.To}");
    }

    private static List<DateOnly> GetPageDates(int pageNumber, int daysPerPage, List<DateOnly> dates)
    {
        var fromIndex = (pageNumber - 1) * daysPerPage;

        var pageDates = dates
            .Skip(fromIndex)
            .Take(daysPerPage)
            .ToList();

        return pageDates;
    }

    private static int CalculateTotalNumberOfPages(int numberOfValidDays, int requestDaysPerPage) =>
        (int)Math.Ceiling((double)numberOfValidDays / requestDaysPerPage);
}
