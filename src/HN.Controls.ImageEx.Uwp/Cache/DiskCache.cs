using System.IO;
using JetBrains.Annotations;
using Windows.Storage;

namespace HN.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 磁盘缓存。
    /// </summary>
    public class DiskCache : DiskCacheBase
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="DiskCache" /> 类的新实例。
        /// </summary>
        public DiskCache() : base(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "ImageExCache"))
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="DiskCache" /> 类的新实例。
        /// </summary>
        /// <param name="cacheFolderPath">缓存文件夹路径。</param>
        public DiskCache([NotNull] string cacheFolderPath) : base(cacheFolderPath)
        {
        }
    }
}
