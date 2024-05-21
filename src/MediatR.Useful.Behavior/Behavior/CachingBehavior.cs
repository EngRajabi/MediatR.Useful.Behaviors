using MediatR.Useful.Behavior.Model;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.Behavior;

public sealed class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheableRequest<TResponse>
    where TResponse : class, new()
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    public CachingBehavior(IDistributedCache distributedCache, IMemoryCache memoryCache)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICacheableRequest<TResponse> cacheable)
        {
            if (string.IsNullOrEmpty(cacheable.CacheKey))
                throw new ArgumentNullException(nameof(request), "null cache key");

            TResponse response = null;
            if (!cacheable.UseMemoryCache)
            {
                var stringResponse = await _distributedCache.GetAsync(cacheable.CacheKey, cancellationToken).ConfigureAwait(false);
                if (stringResponse is not null)
                {
                    var stringCache = Encoding.UTF8.GetString(stringResponse);
                    response = stringCache.ToObject<TResponse>();
                }

                if (response is not null) return response;

                response = await next().ConfigureAwait(false);

                if (response is not null && (cacheable.ConditionForSettingCache is null || cacheable.ConditionForSettingCache(response)))
                    await _distributedCache.SetAsync(cacheable.CacheKey, response.ToJsonUtf8Bytes(), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = cacheable.ConditionExpiration(response)
                    }, cancellationToken).ConfigureAwait(false);

                return response;
            }

            response = _memoryCache.Get<TResponse>(cacheable.CacheKey);

            if (response is not null) return response;

            response = await next().ConfigureAwait(false);

            if (response is not null && (cacheable.ConditionForSettingCache is null || cacheable.ConditionForSettingCache(response)))
                _memoryCache.Set(cacheable.CacheKey, response, cacheable.ConditionExpiration(response));

            return response;
        }

        return await next().ConfigureAwait(false);
    }
}