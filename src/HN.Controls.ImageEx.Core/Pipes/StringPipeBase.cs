using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    public abstract class StringPipeBase<TResult> : PipeBase<TResult> where TResult : class
    {
        protected StringPipeBase(IDesignModeService designModeService) : base(designModeService)
        {
        }

        public override Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is string source)
            {
                context.Current = ToUriSource(source);
            }
            return next(context, cancellationToken);
        }

        protected abstract Uri ToUriSource(string source);
    }
}
