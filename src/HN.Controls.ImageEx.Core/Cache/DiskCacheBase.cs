using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HN.Cache
{
    /// <inheritdoc />
    /// <summary>
    /// 磁盘缓存基础实现。
    /// </summary>
    public abstract class DiskCacheBase : IDiskCache
    {
        /// <summary>
        /// 初始化 <see cref="DiskCacheBase" /> 类的新实例。
        /// </summary>
        /// <param name="cacheFolderPath">缓存文件夹路径。</param>
        /// <exception cref="ArgumentNullException">缓存文件夹路径为 <see langword="null" />。</exception>
        protected DiskCacheBase([NotNull] string cacheFolderPath)
        {
            CacheFolderPath = cacheFolderPath ?? throw new ArgumentNullException(nameof(cacheFolderPath));
        }

        /// <inheritdoc />
        public string CacheFolderPath { get; }

        /// <inheritdoc />
        public ValueTask<long> CalculateAllSizeAsync()
        {
            if (Directory.Exists(CacheFolderPath))
            {
                return new ValueTask<long>(
                    Directory.EnumerateFiles(CacheFolderPath, "*", SearchOption.AllDirectories)
                        .Select(temp => new FileInfo(temp).Length)
                        .Sum());
            }

            return new ValueTask<long>(0L);
        }

        /// <inheritdoc />
        public ValueTask<long> CalculateSizeAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFileName = GetCacheFilePath(key);
            return new ValueTask<long>(new FileInfo(cacheFileName).Length);
        }

        /// <inheritdoc />
        public Task DeleteAllAsync()
        {
            var cacheFolderPath = CacheFolderPath;
            return Task.Run(() =>
            {
                if (Directory.Exists(cacheFolderPath))
                {
                    Directory.Delete(cacheFolderPath, true);
                }
            });
        }

        /// <inheritdoc />
        public Task DeleteAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            return Task.Run(() => File.Delete(cacheFilePath));
        }

        /// <inheritdoc />
        public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            using var fileStream = File.OpenRead(cacheFilePath);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
            return buffer;
        }

        /// <inheritdoc />
        public Task<Stream> GetStreamAsync(string key, CancellationToken cancellationToken = default)
        {
            if (key == null)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            return Task.FromResult<Stream>(File.OpenRead(cacheFilePath));
        }

        /// <inheritdoc />
        public Task<bool> IsExistAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            return Task.FromResult(File.Exists(cacheFilePath));
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

            var cacheFilePath = GetCacheFilePath(key);
            Directory.CreateDirectory(CacheFolderPath);
            using var fileStream = File.Create(cacheFilePath);
            await fileStream.WriteAsync(data, 0, data.Length, cancellationToken);
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

        private string GetCacheFilePath([NotNull] string key)
        {
            var cacheFileName = GetCacheFileName(key);
            return Path.Combine(CacheFolderPath, cacheFileName);
        }
    }
}
