using Practice.Backend.CurrencyConverter.Domain.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Domain.Tests.ExchangeRates;

public sealed class TradingCalendarSpecifications
{
    [Fact]
    public void GetBusinessDays_RangeIsMonToFri_ReturnsFiveDays()
    {
        var monday = new DateOnly(2025, 3, 17);
        var friday = new DateOnly(2025, 3, 21);

        var result = TradingCalendar.GetBusinessDays(monday, friday);

        result.Should().HaveCount(5);
        result.Should().NotContain(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);
    }

    [Fact]
    public void GetBusinessDays_RangeIncludesWeekend_ExcludesWeekendDays()
    {
        var monday = new DateOnly(2025, 3, 17);
        var sunday = new DateOnly(2025, 3, 23);

        var result = TradingCalendar.GetBusinessDays(monday, sunday);

        result.Should().HaveCount(5);
        result.Should().NotContain(d => d.DayOfWeek == DayOfWeek.Saturday);
        result.Should().NotContain(d => d.DayOfWeek == DayOfWeek.Sunday);
    }

    [Fact]
    public void GetBusinessDays_FromAndToAreSameWeekday_ReturnsSingleDay()
    {
        var wednesday = new DateOnly(2025, 3, 19);

        var result = TradingCalendar.GetBusinessDays(wednesday, wednesday);

        result.Should().ContainSingle().Which.Should().Be(wednesday);
    }

    [Fact]
    public void GetBusinessDays_FromAndToAreSameSaturday_ReturnsEmpty()
    {
        var saturday = new DateOnly(2025, 3, 22);

        var result = TradingCalendar.GetBusinessDays(saturday, saturday);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBusinessDays_FromAndToAreSameSunday_ReturnsEmpty()
    {
        var sunday = new DateOnly(2025, 3, 23);

        var result = TradingCalendar.GetBusinessDays(sunday, sunday);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBusinessDays_RangeCoversOnlyWeekend_ReturnsEmpty()
    {
        var saturday = new DateOnly(2025, 3, 22);
        var sunday = new DateOnly(2025, 3, 23);

        var result = TradingCalendar.GetBusinessDays(saturday, sunday);

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetBusinessDays_Always_ReturnsDaysInAscendingOrder()
    {
        var from = new DateOnly(2025, 3, 10);
        var to = new DateOnly(2025, 3, 21);

        var result = TradingCalendar.GetBusinessDays(from, to);

        result.Should().BeInAscendingOrder();
    }

    [Fact]
    public void GetBusinessDays_RangeSpansTwoFullWeeks_ReturnsTenDays()
    {
        var from = new DateOnly(2025, 3, 10);
        var to = new DateOnly(2025, 3, 21);

        var result = TradingCalendar.GetBusinessDays(from, to);

        result.Should().HaveCount(10);
    }
}
