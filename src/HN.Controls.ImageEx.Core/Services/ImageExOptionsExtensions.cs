using System;
using HN.Pipes;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    public static class ImageExOptionsExtensions
    {
        public static IImageExOptions<T> AddPipe<T, TPipe>(this IImageExOptions<T> options) where T : class where TPipe : class, ILoadingPipe<T>
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<ILoadingPipe<T>, TPipe>();
            return options;
        }

        public static IImageExOptions<T> AddService<T, TService>(this IImageExOptions<T> options) where T : class where TService : class
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<TService>();
            return options;
        }

        public static IImageExOptions<T> AddService<T, TService, TImplementation>(this IImageExOptions<T> options) where T : class where TService : class where TImplementation : class, TService
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.Services.AddTransient<TService, TImplementation>();
            return options;
        }
    }
}
