using MediatR;
using MediatR.Useful.Behavior.Model;
using System;

namespace UnitTest.Model;

public sealed class GetUserByRateLimitCommandReq : IRequest<GetUserByRateLimitCommandRes>,
     IRateLimitRequest<GetUserByRateLimitCommandRes>
{
    public GetUserByRateLimitCommandReq(
        int permitLimit,
        Func<GetUserByRateLimitCommandRes, bool> conditionForIncrement,
        Func<GetUserByRateLimitCommandRes, TimeSpan> conditionWindowTime
        )
    {
        PermitLimit = permitLimit;
        ConditionForIncrement = conditionForIncrement;
        ConditionWindowTime = conditionWindowTime;
    }
    public string RateLimitCacheKey => $"cacheKey";
    public int PermitLimit { get; }
    public Func<GetUserByRateLimitCommandRes, bool> ConditionForIncrement { get; }
    public Func<GetUserByRateLimitCommandRes, TimeSpan> ConditionWindowTime { get; }

    public bool UseMemoryCache => false;
}
public sealed class GetUserByRateLimitCommandRes
{
}