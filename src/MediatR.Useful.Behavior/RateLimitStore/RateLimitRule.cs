using System;

namespace MediatR.Useful.Behavior.RateLimitStore;

public class RateLimitRule
{
    public TimeSpan PeriodTimespan { get; set; }
    public int PermitLimit { get; set; }
}
