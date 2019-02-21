using System;
using System.Net.Http;
using System.Windows.Media;
using HN.Cache;
using HN.Pipes;

namespace HN.Services
{
    /// <summary>
    /// <see cref="IImageExOptions{T}" /> 扩展类。
    /// </summary>
    public static class ImageExOptionsExtensions
    {
        public static IImageExOptions<ImageSource> WithDefaultPipes(this IImageExOptions<ImageSource> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.AddPipe<ImageSource, DirectPipe<ImageSource>>();
            options.AddPipe<ImageSource, MemoryCachePipe<ImageSource>>();
            options.AddPipe<ImageSource, StringPipe<ImageSource>>();
            options.AddPipe<ImageSource, DiskCachePipe<ImageSource>>();
            options.AddPipe<ImageSource, UriPipe<ImageSource>>();
            options.AddPipe<ImageSource, ByteArrayPipe<ImageSource>>();
            options.AddPipe<ImageSource, StreamToImageSourcePipe>();
            return options;
        }

        public static IImageExOptions<T> WithDefaultServices<T>(this IImageExOptions<T> options) where T : class
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.AddService<T, IDiskCache, DiskCache>();
            options.AddService<T, IDesignModeService, DesignModeService>();
            options.AddService<T, HttpMessageHandler, HttpClientHandler>();
            return options;
        }
    }
}
