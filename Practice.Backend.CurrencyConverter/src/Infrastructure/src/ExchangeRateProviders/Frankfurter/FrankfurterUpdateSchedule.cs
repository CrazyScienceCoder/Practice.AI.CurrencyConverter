namespace Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

internal static class FrankfurterUpdateSchedule
{
    private static readonly TimeZoneInfo CetZone = TimeZoneInfo.TryFindSystemTimeZoneById("Europe/Berlin", out var iana)
        ? iana
        : TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    private static readonly TimeOnly UpdateTime = new(16, 0);

    public static DateOnly GetExpectedTradingDate(DateTimeOffset utcNow)
    {
        var cetNow = TimeZoneInfo.ConvertTime(utcNow, CetZone);
        var cetDate = DateOnly.FromDateTime(cetNow.DateTime);
        var cetTime = TimeOnly.FromDateTime(cetNow.DateTime);

        var isPostUpdate = cetDate.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday
                           && cetTime >= UpdateTime;

        var candidate = isPostUpdate ? cetDate : cetDate.AddDays(-1);

        while (candidate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            candidate = candidate.AddDays(-1);
        }

        return candidate;
    }
}
