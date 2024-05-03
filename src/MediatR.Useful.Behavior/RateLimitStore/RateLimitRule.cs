using System;

namespace MediatR.Useful.Behavior.Repository;

public class RateLimitRule
{
    public TimeSpan PeriodTimespan { get; set; }
    public int PermitLimit { get; set; }
}
