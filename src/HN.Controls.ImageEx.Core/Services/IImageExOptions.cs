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

        /// <summary>
        /// 获取或设置同时进行的 Http 下载
        /// </summary>
        int MaxHttpDownloadCount { get; set; }
    }

    /// <inheritdoc />
    /// <summary>
    /// ImageEx 配置项。
    /// </summary>
    /// <typeparam name="TSource">数据源目标类型。</typeparam>
    public interface IImageExOptions<TSource> : IImageExOptions where TSource : class
    {
    }
}
