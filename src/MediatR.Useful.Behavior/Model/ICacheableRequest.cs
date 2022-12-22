using System;

namespace MediatR.Useful.Behavior.Model;

public interface ICacheableRequest<T> where T : class, new()
{
    string CacheKey { get; }
    Func<T, DateTimeOffset> ConditionExpiration { get; }
    bool UseMemoryCache { get; }
    virtual Func<T, bool> ConditionFroSetCache => null;

}
public interface ICacheInvalidatorRequest
{
    string CacheKey { get; }
}
