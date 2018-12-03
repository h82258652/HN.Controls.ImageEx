using System.IO;
using Windows.Storage;

namespace HN.Cache
{
    public class DiskCache : DiskCacheBase
    {
        public DiskCache() : base(Path.Combine(ApplicationData.Current.LocalCacheFolder.Path, "ImageExCache"))
        {
        }

        public DiskCache(string cacheFolderPath) : base(cacheFolderPath)
        {
        }
    }
}
