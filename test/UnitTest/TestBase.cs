using MediatR;
using System.Threading.Tasks;

namespace UnitTest;
public abstract class TestBase
{
    protected Task<TResult> RunBehavior<TRequest, TResult>(IPipelineBehavior<TRequest, TResult> behaviour, TRequest command)
        where TRequest : IRequest<TResult>
        where TResult : class, new()
    {
        return behaviour.Handle(command, () => Task.FromResult(new TResult()), default);
    }

    protected Task<TResult> RunBehavior<TRequest, TResult>(IPipelineBehavior<TRequest, TResult> behaviour, TRequest command,
        RequestHandlerDelegate<TResult> handlerDelegate)
        where TRequest : IRequest<TResult>
        where TResult : class, new()
    {
        return behaviour.Handle(command, handlerDelegate, default);
    }
}
