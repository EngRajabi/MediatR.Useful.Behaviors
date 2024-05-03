namespace MediatR.Useful.Behavior.Repository;

public interface IRateLimitStoreStrategy
{
    IRateLimitStore GetRateLimitStore(bool useMemoryCache);
}
