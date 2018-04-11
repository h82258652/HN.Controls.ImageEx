using HN.Extensions;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HN.Pipes
{
    public class UriPipe<TResult> : PipeBase<TResult> where TResult : class
    {
        public override async Task InvokeAsync(LoadingContext<TResult> context, PipeDelegate<TResult> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            var uri = context.Current as Uri;
            if (uri == null)
            {
                await next(context, cancellationToken);
                return;
            }

            if (uri.IsHttp())
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(uri, cancellationToken);
                    response.EnsureSuccessStatusCode();
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    context.Current = bytes;
                    await next(context, cancellationToken);
                    var cacheControl = response.Headers.CacheControl;
                    if (cacheControl != null && !cacheControl.NoCache)
                    {
                        context.HttpResponseBytes = bytes;
                    }
                }
            }
            else if (string.Equals(uri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
            {
                using (var fileStream = File.OpenRead(uri.AbsoluteUri.Substring("file:///".Length)))
                {
                    var buffer = new byte[fileStream.Length];
                    await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    context.Current = buffer;
                }
                await next(context, cancellationToken);
            }
            else
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
}