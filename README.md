[![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://raw.githubusercontent.com/EngRajabi/MediatR.Useful.Behaviors/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/dt/MediatR.Useful.Behavior?label=Nuget.org%20Downloads&style=flat-square&color=blue)](https://www.nuget.org/packages/MediatR.Useful.Behavior)
[![Nuget](https://img.shields.io/nuget/vpre/MediatR.Useful.Behavior.svg?label=NuGet)](https://www.nuget.org/packages/MediatR.Useful.Behavior)

<p align="center">
 <a href="https://www.buymeacoffee.com/mohsenrajabi" target="_blank">
  <img src="https://cdn.buymeacoffee.com/buttons/v2/default-orange.png" height="61" width="194" />
 </a>
</p>

# The best behaviors of MediatR ( Enrich your mediatr project )

We all used mediatr many times. In mediatr, we have a very popular behavior feature that allows us to program aop and cross cutting. We have used it many times. 
But it must have happened to you that you did not combine these behaviors together. 
Instead of implementing these behaviors each time with our own knowledge, I decided to gather these behaviors together so that we can become better every day and have a collective participation in it. 
In this package, I tried to collect the best and most useful behaviors of mediatr and Enrich your mediatr project. and put all the useful requirements of the program in it.

**Package** - [MediatR.Useful.Behavior](https://www.nuget.org/packages/MediatR.Useful.Behavior/)

**Currently, there are 5 very popular and efficient behaviors in this package
- Automatic caching with many features
- Automatic validation
- Automatic logging of unknown errors
- Logging slow commands
- Rate Limit

Add the package to your application using


```bash
dotnet add package MediatR.Useful.Behavior
```
# To use, you can add all behaviors at once. Or add each separately
- AddAllBehaviors: Add all behaviors
- AddCacheBehavior: Add behavior for cache
- AddValidationBehavior: Add validation behavior
- AddUnhandledExceptionBehavior: Add behavior for logging command exceptions
- AddPerformanceBehavior: Add behavior for logging slow commands (commands that take more than 1 second with warning log)
- AddRateLimitBehavior: Add behavior for rate limiting to control the rate of requests
  
How to activate behaviors in the **Startup.cs(or Program.cs)** file

```csharp
// add AddAllBehaviors (cache, validation, unhandled log, performance log)
builder.Services.AddAllBehaviors();
```

By doing this, behaviors are added to the system. Also, all your validations are added to the system

# Cache
Your command must use the ICacheableRequest interface. This interface has several properties that must be set

To use cache, you must first introduce cache services to the system
```csharp
//for in memory cache (RAM)
builder.Services.AddMemoryCache();

//for distribute cache(Redis, Sql, ...)
builder.Services.AddDistributedMemoryCache();
```

```csharp
public sealed class TestCommandRq : IRequest<TestCommandRs>, ICacheableRequest<TestCommandRs>
{
    public long Amount { get; set; }
    public int UserId { get; set; }

    public string CacheKey => $"myKey.{UserId}";
    [JsonIgnore]
    public Func<TestCommandRs, DateTimeOffset> ConditionExpiration => static _ => DateTimeOffset.Now.AddSeconds(10);
    public bool UseMemoryCache => false;
}
```
Properties
- CacheKey:
 CacheKey is your cache key. You can consider a simple key or the key can be a combination of values
- ConditionExpiration:
 With ConditionExpiration you can consider a condition for cache expiration. For example, if the user role was equal to the admin value. The cache value should be 10 minutes. otherwise 10 seconds.
- UseMemoryCache:
 If the specified value of UseMemoryCache is true, it means to use the memory cache. Otherwise, use distributed cache.
- ConditionFroSetCache:
 You can decide based on your command output whether you need to cache or not.
 For example, if the output of the user role list service is empty, it will not be cached.

 Advanced example:
 ```csharp
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
```

# Rate Limit
To implement rate limiting for your commands, you can use the `IRateLimitRequest<T>` interface. 

To use cache, you must first introduce cache services to the system.

```csharp
public sealed class GetUserByRateLimitCommandReq : IRequest<GetUserByRateLimitCommandRes>,
     IRateLimitRequest<GetUserByRateLimitCommandRes>
{
    public string RateLimitCacheKey => $"test.getUserByRateLimit";
    public int PermitLimit => 10;
    public Func<GetUserByRateLimitCommandRes, bool> ConditionForIncrement => _ => true;
    public Func<GetUserByRateLimitCommandRes, TimeSpan> ConditionWindowTime => _ => TimeSpan.FromSeconds(30);
    public bool UseMemoryCache => true;
}
```
Properties
- RateLimitCacheKey:
 RateLimitCacheKey is your cache key. You can consider a simple key or the key can be a combination of values.
- UseMemoryCache:
 If the specified value of UseMemoryCache is true, it means to use the memory cache. Otherwise, use distributed cache.
- PermitLimit:
 The maximum number of requests allowed within the specified time window.
- ConditionForIncrement:
 A function that determines whether a request should count towards the rate limit. If not specified, all requests are counted. 
 For example, you may want to increment the rate limit count only for successful requests.
- ConditionWindowTime:
 With ConditionWindowTime, you can consider a condition for the time window of the rate limit.

 Advanced example:
 ```csharp
public sealed class GetUserByRateLimitCommandReq : IRequest<GetUserByRateLimitCommandRes>,
     IRateLimitRequest<GetUserByRateLimitCommandRes>
{

    public string RateLimitCacheKey => $"test.getUserByRateLimit";
    public int PermitLimit => 10;
    [JsonIgnore]
    public Func<GetUserByRateLimitCommandRes, bool> ConditionForIncrement => rs => rs.Data?.Any() ?? false;
    [JsonIgnore]
    public Func<GetUserByRateLimitCommandRes, TimeSpan> ConditionWindowTime => res =>
        res.IsActive ? TimeSpan.FromSeconds(30) : TimeSpan.FromMinutes(2);
    public bool UseMemoryCache => true;
}
```

# Validation
Before executing your command, the system first executes all your validations.
For validation, you only need to define your model in this way
 ```csharp
public sealed class TestCommandRqValidation : AbstractValidator<TestCommandRq>
{
    public TestCommandRqValidation()
    {
        RuleFor(r => r.Amount).GreaterThan(0);
    }
}
```

# Performance Log
If the command takes more than 1 second. The system records a warning log. With complete specifications of the command and input data.
example log

```csharp
Performance Long Running Request: TestCommandRq 3274 millisecond. {"amount":10000,"userId":0,"cacheKey":"myKey.0","useMemoryCache":false}
```

# Unhandled Log
If an Exception occurs in the command. This behavior records a log with full details.
example log

```csharp
Exception Request: Unhandled Exception for Request TestCommandRq {"amount":0,"userId":0,"cacheKey":"myKey.0","useMemoryCache":false}
      FluentValidation.ValidationException: Validation failed:
       -- Amount: 'Amount' must be greater than '0'. Severity: Error
         at MediatR.Useful.Behavior.Behavior.ValidationBehaviour`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken) in F:\Projects\mediatR-useful-behavior\src\MediatR.Useful.Behavior\Behavior\ValidationBehaviour.cs:line 36
         at MediatR.Useful.Behavior.Behavior.UnhandledExceptionBehaviour`2.Handle(TRequest request, RequestHandlerDelegate`1 next, CancellationToken cancellationToken) in F:\Projects\mediatR-useful-behavior\src\MediatR.Useful.Behavior\Behavior\UnhandledExceptionBehaviour.cs:line 21
```

## Contributing

Create an [issue](https://github.com/EngRajabi/MediatR.Useful.Behaviors/issues/new) if you find a BUG or have a Suggestion or Question. If you want to develop this project :

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request

## Give a Star! ⭐️

If you find this repository useful, please give it a star. Thanks!

## License

MediatR.Useful.Behaviors is Copyright © 2022 [Mohsen Rajabi](https://github.com/EngRajabi) under the MIT License.
