using Practice.Chatbot.CurrencyConverter.Infrastructure.Plugins;

namespace Practice.Chatbot.CurrencyConverter.Infrastructure.Tests.Plugins;

public sealed class PluginResultSpecifications
{
    [Fact]
    public void Success_Always_SetsDataToProvidedValue()
    {
        var result = PluginResult<string>.Success("hello");

        result.Data.Should().Be("hello");
    }

    [Fact]
    public void Success_Always_ErrorIsNull()
    {
        var result = PluginResult<string>.Success("hello");

        result.Error.Should().BeNull();
    }

    [Fact]
    public void Success_Always_IsSuccessIsTrue()
    {
        var result = PluginResult<string>.Success("hello");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Failure_Always_SetsErrorToProvidedMessage()
    {
        const string errorMessage = "Something went wrong.";

        var result = PluginResult<string>.Failure(errorMessage);

        result.Error.Should().Be(errorMessage);
    }

    [Fact]
    public void Failure_Always_DataIsNull()
    {
        var result = PluginResult<string>.Failure("error");

        result.Data.Should().BeNull();
    }

    [Fact]
    public void Failure_Always_IsSuccessIsFalse()
    {
        var result = PluginResult<string>.Failure("error");

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Success_WithComplexType_PreservesData()
    {
        var data = new { Rate = 1.08m, Currency = "USD" };

        var result = PluginResult<object>.Success(data);

        result.Data.Should().BeSameAs(data);
    }
}
