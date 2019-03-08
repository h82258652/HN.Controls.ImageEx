using System;
using System.Net.Http;
using HN.Pipes;
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
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <typeparam name="TPipe">管道类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions<T> AddPipe<T, TPipe>(this IImageExOptions<T> options) where T : class where TPipe : class, ILoadingPipe<T>
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
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions<T> AddService<T, TService>(this IImageExOptions<T> options) where T : class where TService : class
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
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <typeparam name="TService">服务类型。</typeparam>
        /// <typeparam name="TImplementation">实现服务的类型。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions<T> AddService<T, TService, TImplementation>(this IImageExOptions<T> options) where T : class where TService : class where TImplementation : class, TService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<TService, TImplementation>();
            return options;
        }

        /// <summary>
        /// 使用指定的 Http 处理程序。
        /// </summary>
        /// <typeparam name="T">输出值的类型。</typeparam>
        /// <typeparam name="THandler">Http 处理程序。</typeparam>
        /// <param name="options">ImageEx 配置项。</param>
        /// <returns>ImageEx 配置项。</returns>
        public static IImageExOptions<T> UseHttpHandler<T, THandler>(this IImageExOptions<T> options) where T : class where THandler : HttpMessageHandler
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddHttpClient("ImageEx").ConfigurePrimaryHttpMessageHandler<THandler>();
            return options;
        }
    }
}
