namespace HN.Models
{
    /// <summary>
    /// ImageEx 数据源。
    /// </summary>
    public class ImageExSource
    {
        /// <summary>
        /// 数据源的所有帧。
        /// </summary>
        public ImageExFrame[] Frames { get; set; } = default!;

        /// <summary>
        /// 数据源高度。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 数据源动画重复播放次数。
        /// </summary>
        public int RepetitionCount { get; set; }

        /// <summary>
        /// 数据源宽度。
        /// </summary>
        public int Width { get; set; }
    }
}
