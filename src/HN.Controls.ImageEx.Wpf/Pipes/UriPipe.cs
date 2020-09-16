using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using HN.Services;

namespace HN.Pipes
{
    /// <inheritdoc />
    public class UriPipe<TSource> : UriPipeBase<TSource> where TSource : class
    {
        /// <inheritdoc />
        public UriPipe(IDesignModeService designModeService, IHttpClientFactory httpClientFactory) : base(designModeService, httpClientFactory)
        {
        }

        /// <inheritdoc />
        protected override async Task InvokeOtherUriSchemeAsync(ILoadingContext<TSource> context, LoadingPipeDelegate<TSource> next, Uri uri, CancellationToken cancellationToken = default)
        {
            // pack://application:,,,/
            var streamResourceInfo = Application.GetResourceStream(uri);
            if (streamResourceInfo != null)
            {
                context.Current = streamResourceInfo.Stream;
                await next(context, cancellationToken);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
    }
}
