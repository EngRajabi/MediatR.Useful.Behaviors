using MediatR;
using MediatR.Useful.Behavior.Model;

namespace Example.Command;

public sealed class GetUserByRateLimitCommandReq : IRequest<GetUserByRateLimitCommandRes>,
     IRateLimitRequest<GetUserByRateLimitCommandRes>
{
    public string RateLimitCacheKey => $"test.getUserByRateLimit";
    public int PermitLimit => 10;
    public Func<GetUserByRateLimitCommandRes, bool> ConditionForIncrement => _ => true;
    public Func<GetUserByRateLimitCommandRes, TimeSpan> ConditionWindowTime => _ => TimeSpan.FromSeconds(30);

    public bool UseMemoryCache => true;
}
public sealed class GetUserByRateLimitCommandRes
{
}

public sealed class GetUserByRateLimitHandler : IRequestHandler<GetUserByRateLimitCommandReq, GetUserByRateLimitCommandRes>
{
    public Task<GetUserByRateLimitCommandRes> Handle(GetUserByRateLimitCommandReq request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetUserByRateLimitCommandRes());
    }
}