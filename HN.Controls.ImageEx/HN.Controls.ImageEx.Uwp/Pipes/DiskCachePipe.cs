using HN.Cache;
using HN.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HN.Pipes
{
    public class DiskCachePipe<TResult> : PipeBase<TResult> where TResult : class
    {
        private readonly IDiskCache _diskCache;

        public DiskCachePipe(IDiskCache diskCache)
        {
            _diskCache = diskCache;
        }

        public override async Task InvokeAsync(LoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
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
                    async void AsyncAction()
                    {
                        try
                        {
                            await _diskCache.DeleteAsync(cacheKey).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    AsyncAction();
                    throw;
                }
            }
            else
            {
                await next(context, cancellationToken);

                async void AsyncAction()
                {
                    var bytes = context.HttpResponseBytes;
                    if (bytes != null)
                    {
                        try
                        {
                            await _diskCache.SetAsync(cacheKey, bytes, CancellationToken.None).ConfigureAwait(false);
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                AsyncAction();
            }
        }
    }
}