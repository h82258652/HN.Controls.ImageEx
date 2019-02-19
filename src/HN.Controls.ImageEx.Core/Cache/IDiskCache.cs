using System.Threading;
using System.Threading.Tasks;

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
        Task<long> CalculateSizeAsync(string key);

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
        Task DeleteAsync(string key);

        /// <summary>
        /// 获取缓存。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>缓存的值。</returns>
        Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 获取某个缓存是否存在。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <returns>某个缓存是否存在。</returns>
        Task<bool> IsExistAsync(string key);

        /// <summary>
        /// 设置缓存。
        /// </summary>
        /// <param name="key">缓存的键。</param>
        /// <param name="data">缓存的值。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>表示异步设置操作的任务。</returns>
        Task SetAsync(string key, byte[] data, CancellationToken cancellationToken = default(CancellationToken));
    }
}
