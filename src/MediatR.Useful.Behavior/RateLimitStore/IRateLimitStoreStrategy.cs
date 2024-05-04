namespace MediatR.Useful.Behavior.RateLimitStore;

public interface IRateLimitStoreStrategy
{
    IRateLimitStore GetRateLimitStore(bool useMemoryCache);
}
