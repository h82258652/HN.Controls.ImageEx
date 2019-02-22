using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <inheritdoc />
    public class ImageExOptions<T> : IImageExOptions<T> where T : class
    {
        /// <inheritdoc />
        public IServiceCollection Services { get; } = new ServiceCollection();
    }
}
