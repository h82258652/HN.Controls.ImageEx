using System;
using System.IO;
using System.Net.Http;
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
            var scheme = uri.Scheme;
            if (string.Equals(scheme, "ms-appx", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(scheme, "ms-appdata", StringComparison.OrdinalIgnoreCase))
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                context.Current = await file.OpenStreamForReadAsync();
                await next(context, cancellationToken);
            }
            else if (string.Equals(scheme, "ms-resource", StringComparison.OrdinalIgnoreCase))
            {
                var resourceManager = ResourceManager.Current;
                var resourceContext = ResourceContext.GetForCurrentView();
                var candidate = resourceManager.MainResourceMap.GetValue(uri.LocalPath, resourceContext);
                if (candidate != null && candidate.IsMatch)
                {
                    var file = await candidate.GetValueAsFileAsync();
                    context.Current = file.OpenStreamForReadAsync();
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
