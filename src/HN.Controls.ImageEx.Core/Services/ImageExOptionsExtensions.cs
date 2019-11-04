using System;
using System.Net.Http;
using HN.Pipes;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <summary>
    /// <see cref="IImageExOptions{T}" /> 扩展类。
    /// </summary>
    public static class ImageExOptionsExtensions
    {
        /// <summary>
        /// 添加管道。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <param name="pipeType">管道类型。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions AddPipe([NotNull] this IImageExOptions options, Type pipeType)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (pipeType == null)
            {
                throw new ArgumentNullException(nameof(pipeType));
            }

            options.Services.AddTransient(typeof(ILoadingPipe<>), pipeType);
            return options;
        }

        /// <summary>
        /// 添加管道。
        /// </summary>
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <typeparam name="TPipe">管道类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions<T> AddPipe<T, TPipe>([NotNull] this IImageExOptions<T> options) where T : class where TPipe : class, ILoadingPipe<T>
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<ILoadingPipe<T>, TPipe>();
            return options;
        }

        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions AddService<TService>([NotNull] this IImageExOptions options) where TService : class
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<TService>();
            return options;
        }

        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <param name="serviceType">服务类型。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions AddService([NotNull] this IImageExOptions options, Type serviceType)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient(serviceType);
            return options;
        }

        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <typeparam name="TImplementation">实现服务的类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions AddService<TService, TImplementation>([NotNull] this IImageExOptions options) where TService : class where TImplementation : class, TService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<TService, TImplementation>();
            return options;
        }

        /// <summary>
        /// 添加服务。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <param name="serviceType">服务类型。</param>
        /// <param name="implementationType">实现服务的类型。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions AddService([NotNull] this IImageExOptions options, Type serviceType, Type implementationType)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient(serviceType, implementationType);
            return options;
        }

        /// <summary>
        /// 使用指定的 Http 处理程序。
        /// </summary>
        /// <typeparam name="THandler">Http 处理程序。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions UseHttpHandler<THandler>([NotNull] this IImageExOptions options) where THandler : HttpMessageHandler
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<THandler>();
            options.Services.AddHttpClient(Constants.HttpClientName).ConfigurePrimaryHttpMessageHandler(serviceProvider => serviceProvider.GetRequiredService<THandler>());
            return options;
        }

        /// <summary>
        /// 使用指定的 Http 处理程序。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <param name="configureHandler">执行配置的委托。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions UseHttpHandler([NotNull] this IImageExOptions options, Func<HttpMessageHandler> configureHandler)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddHttpClient(Constants.HttpClientName).ConfigurePrimaryHttpMessageHandler(configureHandler);
            return options;
        }

        /// <summary>
        /// 使用指定的 Http 处理程序。
        /// </summary>
        /// <param name="options">ImageEx 配置项。</param>
        /// <param name="configureHandler">执行配置的委托。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions UseHttpHandler([NotNull] this IImageExOptions options, Func<IServiceProvider, HttpMessageHandler> configureHandler)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddHttpClient(Constants.HttpClientName).ConfigurePrimaryHttpMessageHandler(configureHandler);
            return options;
        }
    }
}
