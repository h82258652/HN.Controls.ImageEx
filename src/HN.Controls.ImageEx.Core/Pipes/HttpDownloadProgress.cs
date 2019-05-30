namespace HN.Pipes
{
    /// <summary>
    /// Http 下载进度。
    /// </summary>
    public struct HttpDownloadProgress
    {
        /// <summary>
        /// 获取或设置已接收的字节数。
        /// </summary>
        public ulong BytesReceived { get; set; }

        /// <summary>
        /// 获取下载进度。
        /// </summary>
        public float? Percentage
        {
            get
            {
                if (TotalBytesToReceive.HasValue)
                {
                    return BytesReceived * 1f / TotalBytesToReceive.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// 获取或设置总共需要接收的字节数。
        /// </summary>
        public ulong? TotalBytesToReceive { get; set; }
    }
}
