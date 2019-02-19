using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <summary>
    /// 加载管道调用委托。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    /// <param name="context">加载上下文。</param>
    /// <param name="cancellationToken">要监视取消请求的标记。</param>
    /// <returns>表示异步加载操作的任务。</returns>
    public delegate Task LoadingPipeDelegate<TResult>([NotNull] ILoadingContext<TResult> context, CancellationToken cancellationToken = default(CancellationToken)) where TResult : class;
}
