using FluentAssertions;
using MediatR;
using MediatR.Useful.Behavior;
using MediatR.Useful.Behavior.Behavior;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnitTest.Model;
using Xunit;

namespace UnitTest.Behavior;

public sealed class CachingBehaviorTest : TestBase
{
    private readonly Mock<RequestHandlerDelegate<GetUserPointCommandRes>> _behaviourDelegate;
    private readonly Mock<IDistributedCache> _cahce;
    private readonly Mock<IMemoryCache> _memoryCache;

    public CachingBehaviorTest()
    {
        _behaviourDelegate = new Mock<RequestHandlerDelegate<GetUserPointCommandRes>>();
        _cahce = new Mock<IDistributedCache>();
        _memoryCache = new Mock<IMemoryCache>();
    }


    [Theory]
    [MemberData(nameof(TestCases))]
    public async Task CheckDistributeCache(GetUserPointCommandRes resultBehavior, GetUserPointCommandRes resultCache,
        Func<GetUserPointCommandRes, DateTimeOffset> conditionExpiration,
        Func<GetUserPointCommandRes, bool> conditionFroSetCache,
        int countBehaviorCall, int callGetCache, int callSetCache)
    {
        //Arrange
        var requestCommandRq = new GetUserPointCommandReq(conditionExpiration, conditionFroSetCache);
        var behavior = new CachingBehavior<GetUserPointCommandReq,
            GetUserPointCommandRes>(_cahce.Object, _memoryCache.Object);

        _behaviourDelegate.Setup(r => r.Invoke())
            .ReturnsAsync(resultBehavior);

        _cahce.Setup(r => r.GetAsync(It.IsAny<string>(), default))
            .ReturnsAsync(resultCache.ToJsonUtf8Bytes());


        //Act
        var handle = await RunBehavior(behavior, requestCommandRq, _behaviourDelegate.Object);

        //Assert
        _behaviourDelegate.Verify(r => r.Invoke(), Times.Exactly(countBehaviorCall));
        _cahce.Verify(r => r.GetAsync(
                It.Is<string>(r => r.Equals(requestCommandRq.CacheKey, StringComparison.OrdinalIgnoreCase)), default), Times.Exactly(callGetCache));

        _cahce.Verify(r => r.SetAsync(It.Is<string>(r => r.Equals(requestCommandRq.CacheKey, StringComparison.OrdinalIgnoreCase)),
            resultBehavior.ToJsonUtf8Bytes(), It.Is<DistributedCacheEntryOptions>(r =>
                Math.Abs((r.AbsoluteExpiration.Value - requestCommandRq.ConditionExpiration(resultBehavior)).TotalSeconds) < 10),
            default), Times.Exactly(callSetCache));

        handle.Should().BeEquivalentTo(resultBehavior ?? resultCache);
    }

    public static IEnumerable<object[]> TestCases
    {
#pragma warning disable MA0051 // Method is too long
        get
#pragma warning restore MA0051 // Method is too long
        {
            yield return new object[]
            {
                //method
                null,
                //cache
                null,
                (Func<GetUserPointCommandRes, DateTimeOffset>) (x => DateTimeOffset.Now.AddMinutes(10)),
                null,
                //behavior
                1,
                //get cache
                1,
                //set cache
                0
            };
            yield return new object[]
            {
                //method
                null,
                //cache
                null,
                null,
                null,
                //behavior
                1,
                //get cache
                1,
                //set cache
                0
            };
            yield return new object[]
            {
                //method
                null,
                //cache
                null,
                null,
                null,
                //behavior
                1,
                //get cache
                1,
                //set cache
                0
            };
            yield return new object[]
            {
                //method
                null,
                //cache
                new GetUserPointCommandRes
                {
                    Point = 42.0m
                },
                null,
                null,
                //behavior
                0,
                //get cache
                1,
                //set cache
                0
            };
            yield return new object[]
            {
                //method
                new GetUserPointCommandRes
                {
                    Point = 42.0m
                },
                //cache
                null,
                (Func<GetUserPointCommandRes, DateTimeOffset>) (x => DateTimeOffset.Now.AddMinutes(10)),
                (Func<GetUserPointCommandRes, bool>)(res => true),
                //behavior
                1,
                //get cache
                1,
                //set cache
                1
            };
            yield return new object[]
            {
                //method
                new GetUserPointCommandRes
                {
                    Point = 42.0m
                },
                //cache
                null,
                (Func<GetUserPointCommandRes, DateTimeOffset>) (x => DateTimeOffset.Now.AddMinutes(10)),
                (Func<GetUserPointCommandRes, bool>)(res => false),
                //behavior
                1,
                //get cache
                1,
                //set cache
                0
            };
        }
    }
}

public sealed class GetUserPointCommandRes
{
    public decimal Point { get; set; }
}

