using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace HN.Cache
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct WIN32_FIND_DATA
    {
        public uint itemAttributes;
        public FILETIME creationTime;
        public FILETIME lastAccessTime;
        public FILETIME lastWriteTime;
        public uint fileSizeHigh;
        public uint fileSizeLow;
        public uint reserved0;
        public uint reserved1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string itemName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string alternateFileName;
    }
}
