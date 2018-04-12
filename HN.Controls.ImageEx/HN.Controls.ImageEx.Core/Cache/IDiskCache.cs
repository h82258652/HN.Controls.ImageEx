using System.Threading;
using System.Threading.Tasks;

namespace HN.Cache
{
    public interface IDiskCache
    {
        string CacheFolderPath { get; }

        Task<long> CalculateAllSizeAsync();

        Task<long> CalculateSizeAsync(string key);

        Task DeleteAllAsync();

        Task DeleteAsync(string key);

        Task<byte[]> GetAsync(string key, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> IsExistAsync(string key);

        Task SetAsync(string key, byte[] data, CancellationToken cancellationToken = default(CancellationToken));
    }
}