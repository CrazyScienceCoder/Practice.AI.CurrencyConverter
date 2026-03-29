using Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Plugins;

public sealed class DatePluginSpecifications
{
    [Fact]
    public void GetToday_Always_ReturnsSuccess()
    {
        var result = DatePlugin.GetToday();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void GetToday_Always_ReturnsCurrentDateInIso8601Format()
    {
        var before = DateOnly.FromDateTime(DateTime.UtcNow);

        var result = DatePlugin.GetToday();

        var after = DateOnly.FromDateTime(DateTime.UtcNow);
        var parsed = DateOnly.ParseExact(result.Data!, "yyyy-MM-dd");

        parsed.Should().BeOnOrAfter(before);
        parsed.Should().BeOnOrBefore(after);
    }

    [Fact]
    public void GetToday_Always_DataMatchesYyyyMmDdPattern()
    {
        var result = DatePlugin.GetToday();

        result.Data.Should().MatchRegex(@"^\d{4}-\d{2}-\d{2}$");
    }

    [Fact]
    public void GetToday_Always_ErrorIsNull()
    {
        var result = DatePlugin.GetToday();

        result.Error.Should().BeNull();
    }
}
