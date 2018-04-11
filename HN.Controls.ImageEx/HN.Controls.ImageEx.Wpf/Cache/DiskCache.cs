using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HN.Cache
{
    public class DiskCache : IDiskCache
    {
        public DiskCache()
        {
            CacheFolderPath = Path.Combine(Path.GetTempPath(), "ImageExCache");
        }

        public DiskCache(string cacheFolderPath)
        {
            if (cacheFolderPath == null)
            {
                throw new ArgumentNullException(nameof(cacheFolderPath));
            }

            CacheFolderPath = cacheFolderPath;
        }

        public string CacheFolderPath { get; }

        public Task<long> CalculateAllSizeAsync()
        {
            if (Directory.Exists(CacheFolderPath))
            {
                return Task.FromResult(
                    Directory.EnumerateFiles(CacheFolderPath, "*", SearchOption.AllDirectories)
                        .Select(temp => new FileInfo(temp).Length)
                        .Sum());
            }

            return Task.FromResult(0L);
        }

        public Task<long> CalculateSizeAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFileName = GetCacheFilePath(key);
            return Task.FromResult(new FileInfo(cacheFileName).Length);
        }

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

        public Task DeleteAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            return Task.Run(() => File.Delete(cacheFilePath));
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            using (var fileStream = File.OpenRead(cacheFilePath))
            {
                var buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                return buffer;
            }
        }

        public Task<bool> IsExistAsync(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var cacheFilePath = GetCacheFilePath(key);
            return Task.FromResult(File.Exists(cacheFilePath));
        }

        public async Task SetAsync(string key, byte[] data,
            CancellationToken cancellationToken = default(CancellationToken))
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
            using (var fileStream = File.Create(cacheFilePath))
            {
                await fileStream.WriteAsync(data, 0, data.Length, cancellationToken);
            }
        }

        private string GetCacheFilePath(string key)
        {
            var extension = Path.GetExtension(key) ?? string.Empty;
            using (var md5 = MD5.Create())
            {
                var buffer = Encoding.UTF8.GetBytes(key);
                var hashResult = md5.ComputeHash(buffer);
                var cacheFileName = BitConverter.ToString(hashResult).Replace("-", string.Empty) + extension;
                return Path.Combine(CacheFolderPath, cacheFileName);
            }
        }
    }
}