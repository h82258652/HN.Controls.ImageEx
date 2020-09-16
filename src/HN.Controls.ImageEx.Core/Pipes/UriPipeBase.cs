using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using HN.Extensions;
using HN.Http;
using HN.Services;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see cref="T:System.Uri" /> 类型，则该管道会进行处理。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public abstract class UriPipeBase<TSource> : LoadingPipeBase<TSource> where TSource : class
    {
        private const int BufferSize = 8192;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="UriPipeBase{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        /// <param name="httpClientFactory">HttpClient 工厂。</param>
        protected UriPipeBase(IDesignModeService designModeService, [NotNull] IHttpClientFactory httpClientFactory) : base(designModeService)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        /// <inheritdoc />
        public sealed override async Task InvokeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, CancellationToken cancellationToken = default)
        {
            var uri = context.Current as Uri;
            if (uri == null)
            {
                await next(context, cancellationToken);
                return;
            }

            if (uri.IsHttp())
            {
                try
                {
                    var task = GetDownloadTask(context, uri, cancellationToken);
                    var (bytes, cacheControl) = await task;
                    context.Current = bytes;
                    await next(context, cancellationToken);
                    if (cacheControl != null && !cacheControl.NoCache)
                    {
                        context.HttpResponseBytes = bytes;
                    }
                }
                finally
                {
                    UriPipeInternal.DownloadTasks.Remove(uri);
                }
            }
            else if (uri.IsFile)
            {
                context.Current = File.OpenRead(uri.LocalPath);
                await next(context, cancellationToken);
            }
            else if (string.Equals(uri.Scheme, "data", StringComparison.OrdinalIgnoreCase))
            {
                var base64 = uri.OriginalString;
                const string base64Head = "base64,";
                var base64Index = base64.IndexOf(base64Head, StringComparison.Ordinal);
                var bytes = Convert.FromBase64String(base64.Substring(base64Index + base64Head.Length));
                context.Current = bytes;
                await next(context, cancellationToken);
            }
            else
            {
                await InvokeOtherUriSchemeAsync(context, next, uri, cancellationToken);
            }
        }

        /// <summary>
        /// 执行其它 Uri 协议的加载逻辑。
        /// </summary>
        /// <param name="context">加载上下文。</param>
        /// <param name="next">下一个加载管道的调用委托。</param>
        /// <param name="uri">Uri。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>表示异步加载操作的任务。</returns>
        protected abstract Task InvokeOtherUriSchemeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, Uri uri, CancellationToken cancellationToken = default);

        private async Task<(byte[], CacheControlHeaderValue)> CreateDownloadTask(ILoadingContext<TSource> context, Uri uri, CancellationToken cancellationToken)
        {
            var client = _httpClientFactory.CreateClient(Constants.HttpClientName);
            using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            var cacheControl = response.Headers.CacheControl;
            var contentLength = response.Content.Headers.ContentLength;
            using var stream = await response.Content.ReadAsStreamAsync();
            var buffer = new byte[BufferSize];
            int bytesRead;
            var bytes = new List<byte>();

            var downloadProgress = new HttpDownloadProgress();
            if (contentLength.HasValue)
            {
                downloadProgress.TotalBytesToReceive = (ulong)contentLength.Value;
            }

            context.RaiseDownloadProgressChanged(downloadProgress);

            while ((bytesRead = await stream.ReadAsync(buffer, 0, BufferSize, cancellationToken)) > 0)
            {
                bytes.AddRange(buffer.Take(bytesRead));
                downloadProgress.BytesReceived += (ulong)bytesRead;
                context.RaiseDownloadProgressChanged(downloadProgress);
            }
            
            return (bytes.ToArray(), cacheControl);
        }

        private Task<(byte[], CacheControlHeaderValue)> GetDownloadTask(ILoadingContext<TSource> context, Uri uri, CancellationToken cancellationToken)
        {
            var downloadTasks = UriPipeInternal.DownloadTasks;
            lock (downloadTasks)
            {
                Task<(byte[], CacheControlHeaderValue)> task;
                if (downloadTasks.TryGetValue(uri, out task))
                {
                    return task;
                }

                task = CreateDownloadTask(context, uri, cancellationToken);
                downloadTasks[uri] = task;
                return task;
            }
        }
    }

    internal class UriPipeInternal
    {
        internal static readonly Dictionary<Uri, Task<(byte[], CacheControlHeaderValue)>> DownloadTasks = new Dictionary<Uri, Task<(byte[], CacheControlHeaderValue)>>();
    }
}
