using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    public interface IImageExOptions<T> where T : class
    {
        IServiceCollection Services { get; }
    }
}
