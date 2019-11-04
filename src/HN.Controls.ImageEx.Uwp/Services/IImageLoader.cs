using System.Threading;
using System.Threading.Tasks;

namespace HN.Services
{
    /// <summary>
    /// 图像数据加载类。
    /// </summary>
    public interface IImageLoader
    {
        /// <summary>
        /// 获取图像字节数组形式的数据。
        /// </summary>
        /// <param name="source">图像的源。</param>
        /// <param name="cancellationToken">要监视取消请求的标记。</param>
        /// <returns>图像字节数组形式的数据。</returns>
        Task<byte[]> GetByteArrayAsync(object source, CancellationToken cancellationToken = default);
    }
}
