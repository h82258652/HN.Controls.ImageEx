using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;
using Weakly;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 内存缓存加载管道。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public class MemoryCachePipe<TResult> : LoadingPipeBase<TResult> where TResult : class
    {
        private static readonly WeakValueDictionary<object, TResult> MemoryCache = new WeakValueDictionary<object, TResult>();

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="MemoryCachePipe{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public MemoryCachePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default)
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
