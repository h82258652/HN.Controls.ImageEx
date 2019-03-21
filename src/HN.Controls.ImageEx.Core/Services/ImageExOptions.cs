using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <inheritdoc />
    public class ImageExOptions : IImageExOptions
    {
        /// <inheritdoc />
        public IServiceCollection Services { get; } = new ServiceCollection();
    }

    /// <inheritdoc cref="ImageExOptions" />
    public class ImageExOptions<T> : ImageExOptions, IImageExOptions<T> where T : class
    {
    }
}
