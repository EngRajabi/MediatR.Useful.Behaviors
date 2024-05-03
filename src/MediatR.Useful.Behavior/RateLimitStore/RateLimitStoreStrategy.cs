using System.Collections.Generic;
using System.Linq;

namespace MediatR.Useful.Behavior.Repository;

public sealed class RateLimitStoreStrategy : IRateLimitStoreStrategy
{
    private readonly IEnumerable<IRateLimitStore> _stores;

    public RateLimitStoreStrategy(IEnumerable<IRateLimitStore> stores)
    {
        _stores = stores;
    }

    public IRateLimitStore GetRateLimitStore(bool useMemoryCache) => _stores.First(c => c.UseMemoryCache == useMemoryCache);
}
