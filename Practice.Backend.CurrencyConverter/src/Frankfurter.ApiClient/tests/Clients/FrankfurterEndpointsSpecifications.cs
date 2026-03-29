using Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Clients;

namespace Practice.Backend.CurrencyConverter.Frankfurter.ApiClient.Tests.Clients;

public sealed class FrankfurterEndpointsSpecifications
{
    [Fact]
    public void Latest_Always_ReturnsLatestConstant()
    {
        FrankfurterEndpoints.Latest.Should().Be("latest");
    }

    [Theory]
    [InlineData(2024, 1, 15, "2024-01-15")]
    [InlineData(2023, 12, 31, "2023-12-31")]
    [InlineData(2025, 6, 1, "2025-06-01")]
    [InlineData(2020, 2, 29, "2020-02-29")]
    public void ForDate_ValidDate_ReturnsIso8601FormattedString(int year, int month, int day, string expected)
    {
        var date = new DateOnly(year, month, day);

        var result = FrankfurterEndpoints.ForDate(date);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(2024, 1, 1, 2024, 1, 31, "2024-01-01..2024-01-31")]
    [InlineData(2023, 6, 15, 2023, 12, 31, "2023-06-15..2023-12-31")]
    [InlineData(2025, 3, 10, 2025, 3, 10, "2025-03-10..2025-03-10")]
    public void ForRange_ValidDateRange_ReturnsDotDotSeparatedFormattedString(
        int fromYear, int fromMonth, int fromDay,
        int toYear, int toMonth, int toDay,
        string expected)
    {
        var from = new DateOnly(fromYear, fromMonth, fromDay);
        var to = new DateOnly(toYear, toMonth, toDay);

        var result = FrankfurterEndpoints.ForRange(from, to);

        result.Should().Be(expected);
    }

    [Fact]
    public void ForDate_DateWithSingleDigitMonthAndDay_PadsWithLeadingZeros()
    {
        var date = new DateOnly(2024, 3, 5);

        var result = FrankfurterEndpoints.ForDate(date);

        result.Should().Be("2024-03-05");
    }

    [Fact]
    public void ForRange_DateRange_ContainsDotDotSeparator()
    {
        var from = new DateOnly(2024, 1, 1);
        var to = new DateOnly(2024, 3, 31);

        var result = FrankfurterEndpoints.ForRange(from, to);

        result.Should().Contain("..");
    }
}
