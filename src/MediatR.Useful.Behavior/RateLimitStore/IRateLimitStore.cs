using System.Threading;
using System.Threading.Tasks;

namespace MediatR.Useful.Behavior.RateLimitStore;
public interface IRateLimitStore
{
    bool UseMemoryCache { get; }
    Task IncrementAsync(string key, RateLimitRule rateLimitRule, CancellationToken cancellationToken = default);
    Task<RateLimitCounter> GetAsync(string key, CancellationToken cancellationToken = default);
}
