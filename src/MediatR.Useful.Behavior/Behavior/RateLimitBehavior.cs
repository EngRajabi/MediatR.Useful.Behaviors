using MediatR.Useful.Behavior.Model;
using MediatR.Useful.Behavior.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.Behavior;

public sealed class RateLimitBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IRateLimitRequest<TResponse>
    where TResponse : class, new()
{
    private readonly IRateLimitStoreStrategy _rateLimitStoreStrategy;

    public RateLimitBehavior(IRateLimitStoreStrategy rateLimitStoreStrategy)
    {
        _rateLimitStoreStrategy = rateLimitStoreStrategy;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IRateLimitRequest<TResponse> cacheable)
        {
            if (string.IsNullOrEmpty(cacheable.RateLimitCacheKey))
                throw new ArgumentNullException(nameof(request), "null cache key");

            TResponse response = null;

            var rateLimitStore = _rateLimitStoreStrategy.GetRateLimitStore(request.UseMemoryCache);

            var cacheResult = await rateLimitStore.GetAsync(cacheable.RateLimitCacheKey)
                 .ConfigureAwait(false);

            if (cacheResult is not null && cacheResult.Count >= request.PermitLimit)
                throw new InvalidOperationException("Too many requests. Please try again later.");

            response = await next().ConfigureAwait(false);

            var conditionForIncrement = cacheable.ConditionForIncrement;

            if (conditionForIncrement is null || conditionForIncrement(response))
            {
                RateLimitRule rateLimitRule = new() { PermitLimit = request.PermitLimit, PeriodTimespan = request.ConditionWindowTime(response) };
                await rateLimitStore.IncrementAsync(cacheable.RateLimitCacheKey, rateLimitRule, cancellationToken);
            }

            return response;
        }
        return await next().ConfigureAwait(false);
    }
}