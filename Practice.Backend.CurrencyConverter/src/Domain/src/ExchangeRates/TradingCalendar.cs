namespace Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

public static class TradingCalendar
{
    public static List<DateOnly> GetBusinessDays(DateOnly from, DateOnly to)
    {
        return Enumerable.Range(0, to.DayNumber - from.DayNumber + 1)
            .Select(from.AddDays)
            .Where(d => d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            .ToList();
    }
}
