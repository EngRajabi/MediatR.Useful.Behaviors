using System;
using System.Text.Json.Serialization;

namespace MediatR.Useful.Behavior.Model;

public interface IRateLimitRequest<T> where T : class
{
    bool UseMemoryCache { get; }
    string RateLimitCacheKey { get; }
    int PermitLimit { get; }

    [JsonIgnore]
    virtual Func<T, bool> ConditionForIncrement => null;

    [JsonIgnore]
    Func<T, TimeSpan> ConditionWindowTime { get; }
}
