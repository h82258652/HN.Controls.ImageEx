using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <summary>
    /// 加载管道调用委托。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    /// <param name="context">加载上下文。</param>
    /// <param name="cancellationToken">要监视取消请求的标记。</param>
    /// <returns>表示异步加载操作的任务。</returns>
    public delegate Task LoadingPipeDelegate<out TSource>([NotNull] ILoadingContext<TSource> context, CancellationToken cancellationToken = default) where TSource : class;
}
