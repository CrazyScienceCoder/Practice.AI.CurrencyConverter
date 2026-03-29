using Practice.Backend.CurrencyConverter.Messages.Features.ExchangeRates;

namespace Practice.Backend.CurrencyConverter.Messages.Tests;

public sealed class RequestBaseSpecifications
{
    private sealed record ConcreteRequest : RequestBase;

    [Fact]
    public void Provider_DefaultValue_IsNull()
    {
        var request = new ConcreteRequest();

        request.Provider.Should().BeNull();
    }

    [Fact]
    public void Provider_WhenSet_RetainsValue()
    {
        var request = new ConcreteRequest { Provider = "frankfurter" };

        request.Provider.Should().Be("frankfurter");
    }

    [Fact]
    public void TwoRequests_WithSameProvider_AreEqual()
    {
        var request1 = new ConcreteRequest { Provider = "frankfurter" };
        var request2 = new ConcreteRequest { Provider = "frankfurter" };

        request1.Should().Be(request2);
    }

    [Fact]
    public void TwoRequests_WithDifferentProviders_AreNotEqual()
    {
        var request1 = new ConcreteRequest { Provider = "frankfurter" };
        var request2 = new ConcreteRequest { Provider = "other" };

        request1.Should().NotBe(request2);
    }
}
