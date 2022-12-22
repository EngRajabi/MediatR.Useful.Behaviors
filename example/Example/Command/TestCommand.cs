using FluentValidation;
using MediatR;
using MediatR.Useful.Behavior.Model;

namespace Example.Command;

//validation
public sealed class TestCommandRqValidation : AbstractValidator<TestCommandRq>
{
    public TestCommandRqValidation()
    {
        RuleFor(r => r.Amount).GreaterThan(0);
    }
}

//command
public sealed class TestCommandRq : IRequest<TestCommandRs>, ICacheableRequest<TestCommandRs>
{
    public long Amount { get; set; }

    public string CacheKey => "myKey";
    public Func<TestCommandRs, DateTimeOffset> ConditionExpiration => static _ => DateTimeOffset.Now.AddSeconds(10);
    public bool UseMemoryCache => false;
}

public sealed class TestCommandRs
{
    public DateTime DateTime { get; set; }
}

//handler
public sealed class TestCommandHandler : IRequestHandler<TestCommandRq, TestCommandRs>
{
    public Task<TestCommandRs> Handle(TestCommandRq request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TestCommandRs { DateTime = DateTime.Now });
    }
}
