using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是字节数组，则该管道会进行处理。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public class ByteArrayPipe<TResult> : LoadingPipeBase<TResult> where TResult : class
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ByteArrayPipe{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public ByteArrayPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override Task InvokeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is byte[] bytes)
            {
                context.Current = new MemoryStream(bytes);
            }

            return next(context, cancellationToken);
        }
    }
}
