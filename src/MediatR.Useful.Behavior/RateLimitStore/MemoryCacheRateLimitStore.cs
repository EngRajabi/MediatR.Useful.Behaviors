using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.Repository;
public sealed class MemoryCacheRateLimitStore : IRateLimitStore
{
    public bool UseMemoryCache => true;
    private readonly IMemoryCache _memoryCache;
    public MemoryCacheRateLimitStore(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }


    public async Task IncrementAsync(string key, RateLimitRule rateLimitRule, CancellationToken cancellationToken = default)
    {

        var absoluteExpirationTime = DateTime.UtcNow.Add(rateLimitRule.PeriodTimespan);

        var entryOptions = new MemoryCacheEntryOptions()
             .SetAbsoluteExpiration(absoluteExpirationTime);

        var counter = await _memoryCache.GetOrCreateAsync(key, entry =>
        {
            entry.SetOptions(entryOptions);
            return Task.FromResult(new RateLimitCounter
            {
                Timestamp = absoluteExpirationTime,
                Count = 0
            });
        });

        counter.Count++;


        if (counter.Count > rateLimitRule.PermitLimit)
            return;

        entryOptions.SetAbsoluteExpiration(counter.Timestamp);

        _memoryCache.Set(key, counter, entryOptions);
    }

    public Task<RateLimitCounter> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_memoryCache.TryGetValue(key, out RateLimitCounter cacheEntry))
            return Task.FromResult<RateLimitCounter>(null);

        return Task.FromResult<RateLimitCounter>(cacheEntry);
    }
}