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
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public class MemoryCachePipe<TSource> : LoadingPipeBase<TSource> where TSource : class
    {
        private static readonly WeakValueDictionary<object, TSource> MemoryCache = new WeakValueDictionary<object, TSource>();

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="MemoryCachePipe{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public MemoryCachePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, CancellationToken cancellationToken = default)
        {
            if (IsInDesignMode)
            {
                await next(context, cancellationToken);
                return;
            }

            var cacheKey = context.Current;
            if (cacheKey is string || cacheKey is Uri)
            {
                if (MemoryCache.TryGetValue(cacheKey, out var cacheValue))
                {
                    context.Current = cacheValue;
                    context.AttachSource(cacheValue);
                    return;
                }

                await next(context, cancellationToken);

                if (context.Current is TSource finalValue)
                {
                    MemoryCache[cacheKey] = finalValue;
                }

                return;
            }

            await next(context, cancellationToken);
        }
    }
}
