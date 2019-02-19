using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see langword="string" /> 类型，则该管道会进行处理。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public abstract class StringPipeBase<TResult> : LoadingPipeBase<TResult> where TResult : class
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="StringPipeBase{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        protected StringPipeBase(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override Task InvokeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is string source)
            {
                context.Current = ToUriSource(source);
            }
            return next(context, cancellationToken);
        }

        /// <summary>
        /// 将当前字符串值转换为 <see cref="Uri" /> 对象。
        /// </summary>
        /// <param name="source">当前的值。</param>
        /// <returns>一个 <see cref="Uri" /> 对象。</returns>
        protected abstract Uri ToUriSource(string source);
    }
}
