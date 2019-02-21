using System;
using System.Collections.Generic;
using System.Windows.Media;
using HN.Pipes;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <summary>
    /// ImageEx 服务。
    /// </summary>
    public static class ImageExService
    {
        private static readonly Dictionary<Type, IServiceCollection> Services = new Dictionary<Type, IServiceCollection>();

        static ImageExService()
        {
            ConfigureImageSource(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });
        }

        /// <summary>
        /// 进行配置。
        /// </summary>
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <param name="configure">执行配置的委托。</param>
        public static void Configure<T>(Action<IImageExOptions<T>> configure) where T : class
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var options = new ImageExOptions<T>();

            configure(options);

            Services[typeof(T)] = options.Services;
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
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <returns>输出值的委托调用。</returns>
        public static LoadingPipeDelegate<T> GetHandler<T>() where T : class
        {
            if (!Services.TryGetValue(typeof(T), out var services))
            {
                throw new InvalidOperationException($"type {typeof(T).FullName} is not configure");
            }

            return LoadingPipeBuilder.Build<T>(services);
        }
    }
}
