using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Cache;
using HN.Extensions;
using HN.Services;
using Nito.AsyncEx;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 磁盘加载管道。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public class DiskCachePipe<TResult> : LoadingPipeBase<TResult> where TResult : class
    {
        private readonly IDiskCache _diskCache;

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="DiskCachePipe{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        /// <param name="diskCache">磁盘缓存。</param>
        public DiskCachePipe(IDesignModeService designModeService, IDiskCache diskCache) : base(designModeService)
        {
            _diskCache = diskCache ?? throw new ArgumentNullException(nameof(diskCache));
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, CancellationToken cancellationToken = default)
        {
            if (IsInDesignMode)
            {
                await next(context, cancellationToken);
                return;
            }

            var uri = context.Current as Uri;
            if (uri == null || !uri.IsHttp())
            {
                await next(context, cancellationToken);
                return;
            }

            var cacheKey = uri.AbsoluteUri;
            if (await _diskCache.IsExistAsync(cacheKey))
            {
                context.Current = await _diskCache.GetAsync(cacheKey, cancellationToken);
                try
                {
                    await next(context, cancellationToken);
                }
                catch
                {
                    _diskCache.DeleteAsync(cacheKey).Ignore();
                    throw;
                }
            }
            else
            {
                await next(context, cancellationToken);

                var bytes = context.HttpResponseBytes;
                if (bytes != null)
                {
                    _diskCache.SetAsync(cacheKey, bytes, CancellationToken.None).Ignore();
                }
            }
        }
    }
}
