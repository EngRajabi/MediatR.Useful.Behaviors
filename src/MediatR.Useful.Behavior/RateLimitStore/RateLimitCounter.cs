using System;

namespace MediatR.Useful.Behavior.RateLimitStore;

public class RateLimitCounter
{
    public DateTime Timestamp { get; set; }
    public int Count { get; set; }
}