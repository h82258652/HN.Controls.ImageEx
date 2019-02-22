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
        /// <summary>
        /// 对输出值的类型为 <see cref="ImageSource" /> 使用默认管道。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
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

        /// <summary>
        /// 使用默认服务。
        /// </summary>
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
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
