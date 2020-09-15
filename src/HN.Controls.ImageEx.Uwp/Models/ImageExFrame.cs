using SkiaSharp;

namespace HN.Models
{
    /// <summary>
    /// ImageEx 数据帧。
    /// </summary>
    public class ImageExFrame
    {
        /// <summary>
        /// 该帧显示时长（毫秒）。
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// 该帧的图像。
        /// </summary>
        public SKBitmap Bitmap { get; set; } = default!;
    }
}
