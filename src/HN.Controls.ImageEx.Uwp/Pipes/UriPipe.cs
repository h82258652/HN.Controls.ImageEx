using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using HN.Services;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;

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
            if (string.Equals(uri.Scheme, "ms-appx", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(uri.Scheme, "ms-appdata", StringComparison.OrdinalIgnoreCase))
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                context.Current = await file.OpenStreamForReadAsync();
                await next(context, cancellationToken);
            }
            else if (string.Equals(uri.Scheme, "ms-resource", StringComparison.OrdinalIgnoreCase))
            {
                var resourceManager = ResourceManager.Current;
                var resourceContext = ResourceContext.GetForCurrentView();
                var candidate = resourceManager.MainResourceMap.GetValue(uri.LocalPath, resourceContext);
                if (candidate != null && candidate.IsMatch)
                {
                    var file = await candidate.GetValueAsFileAsync();
                    var buffer = (await FileIO.ReadBufferAsync(file)).ToArray();
                    context.Current = buffer;
                    await next(context, cancellationToken);
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
