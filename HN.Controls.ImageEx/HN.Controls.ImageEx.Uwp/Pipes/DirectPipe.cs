using System.Threading;
using System.Threading.Tasks;

namespace HN.Pipes
{
    public class DirectPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public override Task InvokeAsync(LoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is TResult result)
            {
                context.Result = result;
                return Task.CompletedTask;
            }
            else
            {
                return next(context, cancellationToken);
            }
        }
    }
}