using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    public class UriPipe<TResult> : UriPipeBase<TResult> where TResult : class
    {
        /// <inheritdoc />
        public UriPipe(IDesignModeService designModeService, IHttpClientFactory httpClientFactory) : base(designModeService, httpClientFactory)
        {
        }

        /// <inheritdoc />
        protected override async Task InvokeOtherUriSchemeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, Uri uri, CancellationToken cancellationToken = default)
        {
            // pack://application:,,,/
            var streamResourceInfo = System.Windows.Application.GetResourceStream(uri);
            if (streamResourceInfo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await streamResourceInfo.Stream.CopyToAsync(memoryStream, 81920, cancellationToken);
                    context.Current = memoryStream.ToArray();
                }
                await next(context, cancellationToken);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
