using HN.Pipes;
using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    public class ImageExOptions<T> where T : class
    {
        public IServiceCollection Services { get; } = new ServiceCollection();

        public void AddService<TService>() where TService : class
        {
            Services.AddTransient<TService>();
        }

        public void AddService<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Services.AddTransient<TService, TImplementation>();
        }

        public void AddPipe<TPipe>() where TPipe : class, IPipe<T>
        {
            Services.AddTransient<IPipe<T>, TPipe>();
        }
    }
}