using MediatR.Useful.Behavior.Model;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.Behavior;

public sealed class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ICacheInvalidatorRequest
{
    private readonly IDistributedCache _distributedCache;

    public CacheInvalidationBehavior(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is ICacheInvalidatorRequest invalidator)
        {
            if (string.IsNullOrEmpty(invalidator.CacheKey))
                throw new ArgumentNullException(nameof(request), "null cache key");

            var response = await next().ConfigureAwait(false);

            await _distributedCache.RemoveAsync(invalidator.CacheKey, cancellationToken).ConfigureAwait(false);

            return response;
        }

        return await next().ConfigureAwait(false);
    }
}