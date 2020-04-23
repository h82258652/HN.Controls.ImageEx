using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 加载管道的基本实现。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public abstract class LoadingPipeBase<TSource> : ILoadingPipe<TSource> where TSource : class
    {
        private readonly IDesignModeService _designModeService;

        /// <summary>
        /// 初始化 <see cref="LoadingPipeBase{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        protected LoadingPipeBase([NotNull] IDesignModeService designModeService)
        {
            _designModeService = designModeService ?? throw new ArgumentNullException(nameof(designModeService));
        }

        /// <inheritdoc />
        public bool IsInDesignMode => _designModeService.IsInDesignMode;

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <inheritdoc />
        public abstract Task InvokeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, CancellationToken cancellationToken = default);
    }
}
