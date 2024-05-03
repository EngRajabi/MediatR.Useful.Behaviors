using System;

namespace MediatR.Useful.Behavior.Repository;

public class RateLimitCounter
{
    public DateTime Timestamp { get; set; }
    public int Count { get; set; }
}