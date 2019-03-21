using System;
using System.Net.Http;
using System.Windows.Media;
using HN.Cache;
using HN.Pipes;

namespace HN.Services
{
    /// <summary>
    /// <see cref="IImageExOptions" /> 及 <see cref="IImageExOptions{T}" /> 扩展类。
    /// </summary>
    public static class ImageExOptionsExtensions
    {
        /// <summary>
        /// 使用默认管道。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions WithDefaultPipes(this IImageExOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.AddPipe(typeof(DirectPipe<>));
            options.AddPipe(typeof(MemoryCachePipe<>));
            options.AddPipe(typeof(StringPipe<>));
            options.AddPipe(typeof(DiskCachePipe<>));
            options.AddPipe(typeof(UriPipe<>));
            options.AddPipe(typeof(ByteArrayPipe<>));
            return options;
        }

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

            ((IImageExOptions)options).WithDefaultPipes();
            options.AddPipe<ImageSource, StreamToImageSourcePipe>();
            return options;
        }

        /// <summary>
        /// 使用默认服务。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions WithDefaultServices(this IImageExOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.AddService<IDiskCache, DiskCache>();
            options.AddService<IDesignModeService, DesignModeService>();
            options.UseHttpHandler<HttpClientHandler>();
            return options;
        }
    }
}
