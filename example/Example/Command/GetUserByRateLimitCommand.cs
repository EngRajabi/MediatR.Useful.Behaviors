using MediatR;
using MediatR.Useful.Behavior.Model;
using System.Text.Json.Serialization;

namespace Example.Command;

public sealed class GetUserByRateLimitCommandReq : IRequest<GetUserByRateLimitCommandRes>,
     IRateLimitRequest<GetUserByRateLimitCommandRes>
{
    public string RateLimitCacheKey => $"test.getUserByRateLimit";
    public int PermitLimit => 10;
    [JsonIgnore]
    public Func<GetUserByRateLimitCommandRes, bool> ConditionForIncrement => rs => rs.Data?.Any() ?? false;
    [JsonIgnore]
    public Func<GetUserByRateLimitCommandRes, TimeSpan> ConditionWindowTime => res =>
        res.IsActive ? TimeSpan.FromSeconds(30) : TimeSpan.FromMinutes(2);
    public bool UseMemoryCache => true;
}
public sealed class GetUserByRateLimitCommandRes
{
    public List<int>? Data { get; set; }
    public bool IsActive { get; set; }
}

public sealed class GetUserByRateLimitHandler : IRequestHandler<GetUserByRateLimitCommandReq, GetUserByRateLimitCommandRes>
{
    public Task<GetUserByRateLimitCommandRes> Handle(GetUserByRateLimitCommandReq request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetUserByRateLimitCommandRes());
    }
}