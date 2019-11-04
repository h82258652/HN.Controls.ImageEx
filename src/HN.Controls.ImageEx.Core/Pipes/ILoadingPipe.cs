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
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public interface ILoadingPipe<TResult> : IDisposable where TResult : class
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
        Task InvokeAsync([NotNull] ILoadingContext<TResult> context, [NotNull] LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default);
    }
}
