using System;
using System.Collections.Generic;
using HN.Controls;
using HN.Media;
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
        private static readonly Dictionary<Type, IServiceCollection> Services = new Dictionary<Type, IServiceCollection>();
        private static readonly IServiceCollection SourceSetters = new ServiceCollection();

        static ImageExService()
        {
            ConfigureImageSource(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });

            ConfigureCompositionSurface(options =>
            {
                options.WithDefaultServices();

                options.WithDefaultPipes();
            });

            SetSourceSetter<IImageExSourceSetter, ImageExSourceSetter>();
            SetSourceSetter<IImageBrushExSourceSetter, ImageBrushExSourceSetter>();
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
        /// 进行输出值类型为 <see cref="ICompositionSurface" /> 的配置。
        /// </summary>
        /// <param name="configure">执行配置的委托。</param>
        public static void ConfigureCompositionSurface(Action<IImageExOptions<ICompositionSurface>> configure)
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

        /// <summary>
        /// 获取图像数据源呈现设置器。
        /// </summary>
        /// <typeparam name="TSetter">设置器类型。</typeparam>
        /// <returns>设置器。</returns>
        public static TSetter GetSourceSetter<TSetter>()
        {
            using (var serviceProvider = SourceSetters.BuildServiceProvider())
            {
                return serviceProvider.GetService<TSetter>();
            }
        }

        /// <summary>
        /// 设置图像数据源呈现设置器。
        /// </summary>
        /// <typeparam name="TSetter">设置器类型。</typeparam>
        /// <typeparam name="TImplementation">设置器的实现类型。</typeparam>
        public static void SetSourceSetter<TSetter, TImplementation>() where TSetter : class where TImplementation : class, TSetter
        {
            SourceSetters.AddTransient<TSetter, TImplementation>();
        }
    }
}
