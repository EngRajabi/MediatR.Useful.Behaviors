using FluentValidation;
using MediatR;
using MediatR.Useful.Behavior.Model;
using System.Text.Json.Serialization;

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
    public int UserId { get; set; }

    public string CacheKey => $"myKey.{UserId}";
    [JsonIgnore]
    public Func<TestCommandRs, DateTimeOffset> ConditionExpiration => static _ => DateTimeOffset.Now.AddSeconds(10);
    public bool UseMemoryCache => false;
}

//command
public sealed class TestCommandAdRq : IRequest<TestCommandRs>, ICacheableRequest<TestCommandRs>
{
    public long Amount { get; set; }
    public int UserId { get; set; }

    public string CacheKey => $"myKey.{UserId}";
    [JsonIgnore]
    public Func<TestCommandRs, DateTimeOffset> ConditionExpiration => res =>
        UserId > 0 ? DateTimeOffset.Now.AddMinutes(10) : DateTimeOffset.Now.AddMinutes(1);
    public bool UseMemoryCache => false;
    [JsonIgnore]
    public Func<TestCommandRs, bool> ConditionFroSetCache => rs => rs.Data?.Any() ?? false;
}


public sealed class TestCommandRs
{
    public List<int>? Data { get; set; }
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
