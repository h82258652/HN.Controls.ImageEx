using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Windows.Storage;

namespace HN.Cache
{
    /// <summary>
    /// 磁盘缓存。
    /// </summary>
    public class DiskCache : IDiskCache
    {
        private const string CacheFolderName = "ImageExCache";

        /// <inheritdoc />
        public string CacheFolderPath
        {
            get
            {
                return Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, CacheFolderName);
            }
        }

        /// <inheritdoc />
        public async Task<long> CalculateAllSizeAsync()
        {
            var cacheFolder = await GetCacheFolderAsync();
            if (cacheFolder == null)
            {
                return 0L;
            }

            var cacheFiles = await cacheFolder.GetFilesAsync();

            var context = new CalculateContext();
            var tasks = cacheFiles.Select(async temp =>
            {
                var basicProperties = await temp.GetBasicPropertiesAsync();
                context._totalSize += basicProperties.Size;
            }).ToList();

            await Task.WhenAll(tasks);

            return (long)context._totalSize;
        }

        /// <inheritdoc />
        public async Task<long> CalculateSizeAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFolder = await GetCacheFolderAsync();
            var cacheFileName = GetCacheFileName(key);
            var cacheFile = await cacheFolder.GetFileAsync(cacheFileName);
            var basicProperties = await cacheFile.GetBasicPropertiesAsync();
            return (long)basicProperties.Size;
        }

        /// <inheritdoc />
        public async Task DeleteAllAsync()
        {
            var cacheFolder = await GetCacheFolderAsync();
            if (cacheFolder != null)
            {
                await cacheFolder.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFolder = await GetCacheFolderAsync();
            var cacheFileName = GetCacheFileName(key);
            var cacheFile = await cacheFolder.GetFileAsync(cacheFileName);
            await cacheFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
        }

        /// <inheritdoc />
        public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFolder = await GetCacheFolderAsync();
            var cacheFileName = GetCacheFileName(key);
            var cacheFile = await cacheFolder.GetFileAsync(cacheFileName);
            var buffer = await FileIO.ReadBufferAsync(cacheFile);
            return buffer.ToArray();
        }

        /// <inheritdoc />
        public async Task<bool> IsExistAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFolder = await GetCacheFolderAsync();
            var cacheFileName = GetCacheFileName(key);
            var item = await cacheFolder.TryGetItemAsync(cacheFileName);
            return item != null;
        }

        /// <inheritdoc />
        public async Task SetAsync(string key, byte[] data, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var cacheFolder = await GetCacheFolderAsync();
            var cacheFileName = GetCacheFileName(key);
            var cacheFile = await cacheFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(cacheFile, data);
        }

        private string GetCacheFileName([NotNull] string key)
        {
            var extension = Path.GetExtension(key);
            var invalidCharIndex = extension.IndexOfAny(Path.GetInvalidFileNameChars());
            if (invalidCharIndex > -1)
            {
                extension = extension.Substring(0, invalidCharIndex);
            }

            using var md5 = MD5.Create();
            var buffer = Encoding.UTF8.GetBytes(key);
            var hashResult = md5.ComputeHash(buffer);
            var cacheFileName = BitConverter.ToString(hashResult).Replace("-", string.Empty) + extension;
            return cacheFileName;
        }

        private async Task<StorageFolder> GetCacheFolderAsync()
        {
            var item = await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(CacheFolderName);
            var cacheFolder = item as StorageFolder;
            if (cacheFolder == null)
            {
                cacheFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
            }

            return cacheFolder;
        }

        internal class CalculateContext
        {
            internal ulong _totalSize;
        }
    }
}
