using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows.Media;
using HN.Cache;
using HN.Pipes;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    public static class ImageExService
    {
        private static readonly Dictionary<Type, IServiceCollection> Services = new Dictionary<Type, IServiceCollection>();

        static ImageExService()
        {
            Configure<ImageSource>(options =>
            {
                options.AddService<IDiskCache, DiskCache>();
                options.AddService<IDesignModeService, DesignModeService>();
                options.AddService<HttpMessageHandler, HttpClientHandler>();

                options.AddPipe<DirectPipe<ImageSource>>();
                options.AddPipe<MemoryCachePipe<ImageSource>>();
                options.AddPipe<StringPipe<ImageSource>>();
                options.AddPipe<DiskCachePipe<ImageSource>>();
                options.AddPipe<UriPipe<ImageSource>>();
                options.AddPipe<ByteArrayPipe<ImageSource>>();
                options.AddPipe<StreamToImageSourcePipe>();
            });
        }

        public static void Configure<T>(Action<ImageExOptions<T>> configure) where T : class
        {
            var options = new ImageExOptions<T>();

            configure(options);

            Services[typeof(T)] = options.Services;
        }

        public static void ConfigureImageEx(Action<ImageExOptions<ImageSource>> configure)
        {
            Configure(configure);
        }

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
