using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;
using Weakly;

namespace HN.Pipes
{
    public class MemoryCachePipe<TResult> : PipeBase<TResult> where TResult : class
    {
        private static readonly WeakValueDictionary<object, TResult> MemoryCache = new WeakValueDictionary<object, TResult>();

        public MemoryCachePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        public override async Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (IsInDesignMode)
            {
                await next(context, cancellationToken);
                return;
            }

            var source = context.Current;
            if (source is string || source is Uri)
            {
                if (MemoryCache.TryGetValue(source, out var cache))
                {
                    context.Result = cache;
                    return;
                }

                await next(context, cancellationToken);

                var result = context.Result;
                if (result != null)
                {
                    MemoryCache[source] = result;
                }

                return;
            }

            await next(context, cancellationToken);
        }
    }
}
