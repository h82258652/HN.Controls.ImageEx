using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see langword="byte" /> 数组类型，则该管道会进行处理。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public class ByteArrayPipe<TSource> : LoadingPipeBase<TSource> where TSource : class
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ByteArrayPipe{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public ByteArrayPipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override Task InvokeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, CancellationToken cancellationToken = default)
        {
            if (context.Current is byte[] bytes)
            {
                context.Current = new MemoryStream(bytes);
            }

            return next(context, cancellationToken);
        }
    }
}
