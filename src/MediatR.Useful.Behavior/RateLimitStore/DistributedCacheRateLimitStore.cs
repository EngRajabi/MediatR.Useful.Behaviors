using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.Repository;

public sealed class DistributedCacheRateLimitStore : IRateLimitStore
{
    public bool UseMemoryCache => false;
    private readonly IDistributedCache _distributedCache;
    public DistributedCacheRateLimitStore(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<RateLimitCounter> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var stored = await _distributedCache.GetStringAsync(key, cancellationToken);

        if (!string.IsNullOrEmpty(stored))
            return stored.ToObject<RateLimitCounter>();

        return null;
    }

    public async Task IncrementAsync(string key, RateLimitRule rateLimitRule, CancellationToken cancellationToken = default)
    {
        var cacheEntry = await GetAsync(key, cancellationToken);

        var counter = new RateLimitCounter
        {
            Timestamp = DateTime.UtcNow.Add(rateLimitRule.PeriodTimespan),
            Count = 1
        };

        if (cacheEntry is not null)
        {
            cacheEntry.Count++;

            if (cacheEntry.Count > rateLimitRule.PermitLimit)
                return;

            counter = new RateLimitCounter
            {
                Timestamp = cacheEntry.Timestamp,
                Count = cacheEntry.Count
            };
        }

        await _distributedCache.SetStringAsync(key, counter.ToJson(), new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = counter.Timestamp
        }, cancellationToken).ConfigureAwait(false);
    }
}