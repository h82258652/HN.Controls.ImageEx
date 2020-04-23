using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 加载管道。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public interface ILoadingPipe<TSource> : IDisposable where TSource : class
    {
        /// <summary>
        /// 获取当前是否在设计模式下。
        /// </summary>
        bool IsInDesignMode { get; }

        /// <summary>
        /// 执行管道加载逻辑。
        /// </summary>
        /// <param name="context">加载上下文。</param>
        /// <param name="next">下一个加载管道的调用委托。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>表示异步加载操作的任务。</returns>
        Task InvokeAsync([NotNull] ILoadingContext<TSource> context, [NotNull] LoadingPipeDelegate<TSource> next, CancellationToken cancellationToken = default);
    }
}
