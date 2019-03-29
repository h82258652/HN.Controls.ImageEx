using System;

namespace HN.Utils
{
    /// <summary>
    /// Gif 图像帧元数据。
    /// </summary>
    public class GifFrameMetadata
    {
        /// <summary>
        /// 该帧的持续时间。
        /// </summary>
        public TimeSpan Delay { get; set; }

        /// <summary>
        /// 该帧的处理方式。
        /// </summary>
        public FrameDisposalMethod Disposal { get; set; }

        /// <summary>
        /// 该帧图像的高度。
        /// </summary>
        public ushort Height { get; set; }

        /// <summary>
        /// 该帧图像距离左边缘的距离。
        /// </summary>
        public ushort Left { get; set; }

        /// <summary>
        /// 该帧图像距离上边缘的距离。
        /// </summary>
        public ushort Top { get; set; }

        /// <summary>
        /// 该帧图像的宽度。
        /// </summary>
        public ushort Width { get; set; }
    }
}
