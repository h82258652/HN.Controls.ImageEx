using System;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;
using Windows.Storage;

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
        protected override async Task InvokeOtherUriSchemeAsync(ILoadingContext<TResult> context, LoadingPipeDelegate<TResult> next, Uri uri, CancellationToken cancellationToken = default(CancellationToken))
        {
            // ms-appx:/ or ms-appdata:/
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = (await FileIO.ReadBufferAsync(file)).ToArray();
            context.Current = buffer;
            await next(context, cancellationToken);
        }
    }
}
