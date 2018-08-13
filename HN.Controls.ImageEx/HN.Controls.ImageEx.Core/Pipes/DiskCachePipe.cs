using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Cache;
using HN.Extensions;
using HN.Services;
using Microsoft.VisualStudio.Threading;

namespace HN.Pipes
{
    public class DiskCachePipe<TResult> : PipeBase<TResult> where TResult : class
    {
        private readonly IDiskCache _diskCache;

        public DiskCachePipe(IDesignModeService designModeService, IDiskCache diskCache) : base(designModeService)
        {
            _diskCache = diskCache ?? throw new ArgumentNullException(nameof(diskCache));
        }

        public override async Task InvokeAsync(ILoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
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
                    _diskCache.DeleteAsync(cacheKey).Forget();
                    throw;
                }
            }
            else
            {
                await next(context, cancellationToken);

                var bytes = context.HttpResponseBytes;
                if (bytes != null)
                {
                    _diskCache.SetAsync(cacheKey, bytes, CancellationToken.None).Forget();
                }
            }
        }
    }
}