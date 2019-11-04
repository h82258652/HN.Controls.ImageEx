using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Cache
{
    /// <summary>
    /// 磁盘缓存。
    /// </summary>
    public interface IDiskCache
    {
        /// <summary>
        /// 获取缓存文件夹路径。
        /// </summary>
        [NotNull]
        string CacheFolderPath { get; }

        /// <summary>
        /// 计算所有缓存的大小。单位：字节。
        /// </summary>
        /// <returns>所有缓存的大小。单位：字节。</returns>
        Task<long> CalculateAllSizeAsync();

        /// <summary>
        /// 计算某个缓存的大小。单位：字节。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <returns>某个缓存的大小。单位：字节。</returns>
        Task<long> CalculateSizeAsync([NotNull] string key);

        /// <summary>
        /// 删除所有缓存。
        /// </summary>
        /// <returns>表示异步删除操作的任务。</returns>
        Task DeleteAllAsync();

        /// <summary>
        /// 删除某个缓存。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <returns>表示异步删除操作的任务。</returns>
        Task DeleteAsync([NotNull] string key);

        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>缓存的值。</returns>
        Task<byte[]> GetAsync([NotNull] string key, CancellationToken cancellationToken = default);

        /// <summary>
        /// 获取某个缓存是否存在。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <returns>某个缓存是否存在。</returns>
        Task<bool> IsExistAsync([NotNull] string key);

        /// <summary>
        /// 设置缓存。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="data">缓存的值。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>表示异步设置操作的任务。</returns>
        Task SetAsync([NotNull] string key, [NotNull] byte[] data, CancellationToken cancellationToken = default);
    }
}
