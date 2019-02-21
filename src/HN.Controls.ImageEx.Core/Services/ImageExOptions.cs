using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    public class ImageExOptions<T> : IImageExOptions<T> where T : class
    {
        public IServiceCollection Services { get; } = new ServiceCollection();
    }
}
