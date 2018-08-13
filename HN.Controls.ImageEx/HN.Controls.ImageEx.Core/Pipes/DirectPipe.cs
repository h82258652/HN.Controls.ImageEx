using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    public class DirectPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public DirectPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        public override Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is TResult result)
            {
                context.Result = result;
                return Task.CompletedTask;
            }

            return next(context, cancellationToken);
        }
    }
}