using System;
using System.Text.Json.Serialization;

namespace MediatR.Useful.Behavior.Model;

public interface ICacheableRequest<T> where T : class, new()
{
    string CacheKey { get; }

    [JsonIgnore]
    Func<T, DateTimeOffset> ConditionExpiration { get; }

    bool UseMemoryCache { get; }

    [JsonIgnore]
    virtual Func<T, bool> ConditionForSettingCache => null;

}
public interface ICacheInvalidatorRequest
{
    string CacheKey { get; }
}
