using FluentAssertions;
using MediatR;
using MediatR.Useful.Behavior.Behavior;
using MediatR.Useful.Behavior.RateLimitStore;
using Moq;
using System;
using System.Threading.Tasks;
using UnitTest.Model;
using Xunit;

namespace UnitTest.Behavior;

public sealed class RateLimitBehaviorTest : TestBase
{
    private readonly Mock<IRateLimitStoreStrategy> _rateLimitStoreStrategy;
    private readonly Mock<RequestHandlerDelegate<GetUserByRateLimitCommandRes>> _behaviourDelegate;
    private readonly RateLimitBehavior<GetUserByRateLimitCommandReq, GetUserByRateLimitCommandRes> _behavior;
    private readonly GetUserByRateLimitCommandRes _commandRes;
    private readonly Mock<IRateLimitStore> _rateLimitStore;
    public RateLimitBehaviorTest()
    {
        _rateLimitStore = new();
        _rateLimitStoreStrategy = new();
        _commandRes = new();
        _behaviourDelegate = new();

        _behaviourDelegate.Setup(r => r.Invoke())
            .ReturnsAsync(_commandRes);
        _rateLimitStoreStrategy.Setup(c => c.GetRateLimitStore(It.IsAny<bool>())).Returns(_rateLimitStore.Object);

        _behavior = new(_rateLimitStoreStrategy.Object);
    }

    [Fact]
    public async Task When_Cache_Is_Null_And_ConditionForIncrement_Is_False_Success()
    {
        // Arrange
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default));

        var commandReq = new GetUserByRateLimitCommandReq(5, _ => false, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.IsAny<string>(),
            It.IsAny<RateLimitRule>(),
            default), Times.Never);
    }

    [Fact]
    public async Task When_Cache_Is_Null_And_ConditionForIncrement_Is_True_Success()
    {
        // Arrange
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default));

        var commandReq = new GetUserByRateLimitCommandReq(5, _ => true, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)),
            It.Is<RateLimitRule>(c => c.PeriodTimespan == commandReq.ConditionWindowTime(_commandRes) && c.PermitLimit == commandReq.PermitLimit),
            default), Times.Once);
    }

    [Fact]
    public async Task When_Cache_Is_Null_And_ConditionForIncrement_Is_Null_Success()
    {
        // Arrange
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default));

        var commandReq = new GetUserByRateLimitCommandReq(5, null, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);
        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)),
            It.Is<RateLimitRule>(c => c.PeriodTimespan == commandReq.ConditionWindowTime(_commandRes) && c.PermitLimit == commandReq.PermitLimit),
            default), Times.Once);
    }

    [Fact]
    public async Task When_Cache_HasValue_And_Counter_In_Cache_Is_Less_Than_PermitLimit_And_ConditionForIncrement_Is_Null_Success()
    {
        // Arrange
        var rateLimitCounter = new RateLimitCounter()
        {
            Count = 2,
        };
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(rateLimitCounter);

        var commandReq = new GetUserByRateLimitCommandReq(5, null, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)),
            It.Is<RateLimitRule>(c => c.PeriodTimespan == commandReq.ConditionWindowTime(_commandRes) && c.PermitLimit == commandReq.PermitLimit),
            default), Times.Once);
    }

    [Fact]
    public async Task When_Cache_HasValue_And_Counter_In_Cache_Is_Less_Than_PermitLimit_And_ConditionForIncrement_Is_True_Success()
    {
        // Arrange
        var rateLimitCounter = new RateLimitCounter()
        {
            Count = 2,
        };
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(rateLimitCounter);

        var commandReq = new GetUserByRateLimitCommandReq(5, _ => true, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)),
            It.Is<RateLimitRule>(c => c.PeriodTimespan == commandReq.ConditionWindowTime(_commandRes) && c.PermitLimit == commandReq.PermitLimit),
            default), Times.Once);
    }

    [Fact]
    public async Task When_Cache_HasValue_And_Counter_In_Cache_Is_Less_Than_PermitLimit_And_ConditionForIncrement_Is_False_Success()
    {
        // Arrange
        var rateLimitCounter = new RateLimitCounter()
        {
            Count = 2,
        };
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(rateLimitCounter);

        var commandReq = new GetUserByRateLimitCommandReq(5, _ => false, _ => TimeSpan.FromSeconds(30));
        // Act

        var handle = await RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert
        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.IsAny<string>(),
            It.IsAny<RateLimitRule>(),
            default), Times.Never);
    }

    [Fact]
    public async Task _When_Cache_Is_Not_Null_And_Counter_In_Cache_Is_Grather_Than_PermitLimit_Fail()
    {
        // Arrange
        var rateLimitCounter = new RateLimitCounter()
        {
            Count = 6,
        };
        _rateLimitStore.Setup(r => r.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(rateLimitCounter);

        var commandReq = new GetUserByRateLimitCommandReq(5, _ => false, _ => TimeSpan.FromSeconds(30));
        //act

        var act = () => RunBehaviour(_behavior, commandReq, _behaviourDelegate.Object);
        // Assert

        await act.Should().ThrowExactlyAsync<InvalidOperationException>().WithMessage("Too many requests. Please try again later.");


        _rateLimitStoreStrategy.Verify(r => r.GetRateLimitStore(It.Is<bool>(c => c == commandReq.UseMemoryCache)), Times.Once);
        _rateLimitStore.Verify(r => r.GetAsync(It.Is<string>(c => c.Equals(commandReq.RateLimitCacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Once);

        _rateLimitStore.Verify(r => r.IncrementAsync(
            It.IsAny<string>(),
            It.IsAny<RateLimitRule>(),
            default), Times.Never);
    }
}

