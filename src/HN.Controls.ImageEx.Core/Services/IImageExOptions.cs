using Microsoft.Extensions.DependencyInjection;

namespace HN.Services
{
    /// <summary>
    /// ImageEx 配置项。
    /// </summary>
    public interface IImageExOptions
    {
        /// <summary>
        /// 获取管道执行过程中使用到的服务集合。
        /// </summary>
        /// <returns>
        /// 管道执行过程中使用到的服务集合。
        /// </returns>
        IServiceCollection Services { get; }
    }

    /// <inheritdoc />
    /// <summary>
    /// ImageEx 配置项。
    /// </summary>
    /// <typeparam name="T">输出值的类型。</typeparam>
    public interface IImageExOptions<T> : IImageExOptions where T : class
    {
    }
}
