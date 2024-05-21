using FluentValidation;
using MediatR;
using MediatR.Useful.Behavior.Model;
using System;
using UnitTest.Behavior;

namespace UnitTest.Model;

public sealed class GetUserPointCommandReq : IRequest<GetUserPointCommandRes>,
    ICacheableRequest<GetUserPointCommandRes>
{
    public GetUserPointCommandReq()
    {

    }

    public GetUserPointCommandReq(Func<GetUserPointCommandRes, DateTimeOffset> conditionExpiration,
        Func<GetUserPointCommandRes, bool> conditionFroSetCache, bool useMemoryCache = false)
    {
        ConditionExpiration = conditionExpiration;
        ConditionForSettingCache = conditionFroSetCache;
        UseMemoryCache = useMemoryCache;
        UserId = Guid.NewGuid().ToString();
    }

    public string UserId { get; set; }
    public long Amount { get; set; }
    public string CacheKey => $"test.userpoint.{UserId}";
    public Func<GetUserPointCommandRes, DateTimeOffset> ConditionExpiration { get; }
    public bool UseMemoryCache { get; }
    public Func<GetUserPointCommandRes, bool> ConditionForSettingCache { get; }
}

public sealed class GetUserPointCommandReqValidator : AbstractValidator<GetUserPointCommandReq>
{
    public GetUserPointCommandReqValidator()
    {
        RuleFor(r => r.UserId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThanOrEqualTo(100);
    }
}