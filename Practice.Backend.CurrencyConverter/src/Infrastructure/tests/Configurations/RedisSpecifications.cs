using Practice.Backend.CurrencyConverter.Infrastructure.Configurations;

namespace Practice.Backend.CurrencyConverter.Infrastructure.Tests.Configurations;

public sealed class RedisSpecifications
{
    [Fact]
    public void ConnectionString_WhenSet_ReturnsAssignedValue()
    {
        var redis = new Redis { ConnectionString = "localhost:6379", InstanceName = "test" };

        redis.ConnectionString.Should().Be("localhost:6379");
    }

    [Fact]
    public void InstanceName_WhenSet_ReturnsAssignedValue()
    {
        var redis = new Redis { ConnectionString = "localhost:6379", InstanceName = "my-instance" };

        redis.InstanceName.Should().Be("my-instance");
    }
}
