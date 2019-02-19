using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值已经是目标类型，则结束管道。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public class DirectPipe<TResult> : LoadingPipeBase<TResult> where TResult : class
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="DirectPipe{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public DirectPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override Task InvokeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
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
