using System.IO;

namespace HN.Cache
{
    public class DiskCache : DiskCacheBase
    {
        public DiskCache() : base(Path.Combine(Path.GetTempPath(), "ImageExCache"))
        {
        }

        public DiskCache(string cacheFolderPath) : base(cacheFolderPath)
        {
        }
    }
}