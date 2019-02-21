using System;
using System.Net.Http;
using HN.Cache;
using HN.Pipes;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace HN.Services
{
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

        public static IImageExOptions<ICompositionSurface> WithDefaultPipes(this IImageExOptions<ICompositionSurface> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.AddPipe<ICompositionSurface, DirectPipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, MemoryCachePipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, StringPipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, DiskCachePipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, UriPipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, ByteArrayPipe<ICompositionSurface>>();
            options.AddPipe<ICompositionSurface, StreamToCompositionSurfacePipe>();
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
