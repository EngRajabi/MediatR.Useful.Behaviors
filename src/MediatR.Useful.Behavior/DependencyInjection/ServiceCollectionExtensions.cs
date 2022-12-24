using FluentValidation;
using MediatR.Useful.Behavior.Behavior;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MediatR.Useful.Behavior.DependencyInjection;
public static class ServiceCollectionExtensions
{
    public static void AddAllBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

        var assemblys = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(selector => selector
            .FromAssemblies(assemblys)
            .AddClasses(filter => filter.AssignableTo(typeof(IValidator<>)).Where(r =>
                !r.IsAbstract && r.IsClass && !r.IsGenericType && r.IsPublic
            ))
            .AsImplementedInterfaces());
    }

    public static void AddCacheBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));
    }

    public static void AddValidationBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
    }

    public static void AddUnhandledExceptionBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
    }

    public static void AddPerformanceBehavior(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
    }
}
