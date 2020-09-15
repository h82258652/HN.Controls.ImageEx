using System;
using System.Collections.Generic;
using System.Threading;
using HN.Models;
using HN.Pipes;
using Microsoft.Extensions.DependencyInjection;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace HN.Services
{
    /// <summary>
    /// ImageEx 服务。
    /// </summary>
    public static class ImageExService
    {
        private static readonly IDictionary<Type, IServiceCollection> Services = new Dictionary<Type, IServiceCollection>();

        private static SemaphoreSlim? _throttler;
        private static int _throttlerInitialCount;

        static ImageExService()
        {
            ConfigureByteArray(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });

            ConfigureImageSource(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });

            ConfigureImageExSource(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });

            ConfigureCompositionSurface(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });
        }

        /// <summary>
        /// 进行配置。
        /// </summary>
        /// <typeparam name="TSource">目标源的类型。</typeparam>
        /// <param name="configure">执行配置的委托。</param>
        public static void Configure<TSource>(Action<IImageExOptions<TSource>> configure) where TSource : class
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new ImageExOptions<TSource>();

            configure(options);

            if (_throttler == null || _throttlerInitialCount != options.MaxHttpDownloadCount)
            {
                _throttler = new SemaphoreSlim(options.MaxHttpDownloadCount);
                _throttlerInitialCount = options.MaxHttpDownloadCount;
            }

            options.Services.AddSingleton(_throttler);
            Services[typeof(TSource)] = options.Services;
        }

        /// <summary>
        /// 进行输出值类型为字节数组的配置。
        /// </summary>
        /// <param name="configure">执行配置的委托。</param>
        public static void ConfigureByteArray(Action<IImageExOptions<byte[]>> configure)
        {
            Configure(configure);
        }

        /// <summary>
        /// 进行输出值类型为 <see cref="ICompositionSurface" /> 的配置。
        /// </summary>
        /// <param name="configure">执行配置的委托。</param>
        public static void ConfigureCompositionSurface(Action<IImageExOptions<ICompositionSurface>> configure)
        {
            Configure(configure);
        }

        /// <summary>
        /// 进行输出值类型为 <see cref="ImageExSource" /> 的配置。
        /// </summary>
        /// <param name="configure">执行配置的委托。</param>
        public static void ConfigureImageExSource(Action<IImageExOptions<ImageExSource>> configure)
        {
            Configure(configure);
        }

        /// <summary>
        /// 进行输出值类型为 <see cref="ImageSource" /> 的配置。
        /// </summary>
        /// <param name="configure">执行配置的委托。</param>
        public static void ConfigureImageSource(Action<IImageExOptions<ImageSource>> configure)
        {
            Configure(configure);
        }

        /// <summary>
        /// 获取管道组装后，输出值的委托调用。
        /// </summary>
        /// <typeparam name="TSource">目标源的类型。</typeparam>
        /// <returns>输出值的委托调用。</returns>
        public static LoadingPipeDelegate<TSource> GetHandler<TSource>() where TSource : class
        {
            if (!Services.TryGetValue(typeof(TSource), out var services))
            {
                throw new InvalidOperationException($"type {typeof(TSource).FullName} is not configure");
            }

            return LoadingPipeBuilder.Build<TSource>(services);
        }
    }
}
