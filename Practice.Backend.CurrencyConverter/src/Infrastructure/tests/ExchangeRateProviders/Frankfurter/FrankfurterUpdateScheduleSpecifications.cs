using Practice.Backend.CurrencyConverter.Infrastructure.ExchangeRateProviders.Frankfurter;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.ExchangeRateProviders.Frankfurter;

public sealed class FrankfurterUpdateScheduleSpecifications
{
    [Fact]
    public void GetExpectedTradingDate_WeekdayBeforeUpdate_ReturnsYesterdayWeekday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 16, 14, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 15));
    }

    [Fact]
    public void GetExpectedTradingDate_WeekdayAfterUpdate_ReturnsTodayWeekday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 16, 16, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 16));
    }

    [Fact]
    public void GetExpectedTradingDate_WeekdayExactlyAtUpdateTime_ReturnsTodayWeekday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 17, 15, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 17));
    }

    [Fact]
    public void GetExpectedTradingDate_MondayBeforeUpdate_ReturnsLastFriday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 15, 8, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 12));
    }

    [Fact]
    public void GetExpectedTradingDate_Saturday_ReturnsLastFriday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 13, 10, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 12));
    }

    [Fact]
    public void GetExpectedTradingDate_Sunday_ReturnsLastFriday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 14, 10, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 12));
    }

    [Fact]
    public void GetExpectedTradingDate_FridayAfterUpdate_ReturnsFriday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 12, 16, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 12));
    }

    [Fact]
    public void GetExpectedTradingDate_FridayBeforeUpdate_ReturnsThursday()
    {
        var utcNow = new DateTimeOffset(2024, 1, 12, 10, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 1, 11));
    }

    [Fact]
    public void GetExpectedTradingDate_SummerTime_AccountsForCest()
    {
        var utcNow = new DateTimeOffset(2024, 7, 10, 13, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 7, 9));
    }

    [Fact]
    public void GetExpectedTradingDate_SummerTimeAfterUpdate_AccountsForCest()
    {
        var utcNow = new DateTimeOffset(2024, 7, 10, 14, 0, 0, TimeSpan.Zero);

        var result = FrankfurterUpdateSchedule.GetExpectedTradingDate(utcNow);

        result.Should().Be(new DateOnly(2024, 7, 10));
    }
}
