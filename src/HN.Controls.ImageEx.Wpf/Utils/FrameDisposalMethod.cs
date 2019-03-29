namespace HN.Utils
{
    /// <summary>
    /// 图像帧的处理方式。
    /// </summary>
    public enum FrameDisposalMethod
    {
        /// <summary>
        /// 不做处理。
        /// </summary>
        None = 0,

        /// <summary>
        /// 保留图像。
        /// </summary>
        DoNotDispose = 1,

        /// <summary>
        /// 恢复为背景色。
        /// </summary>
        RestoreToBackgroundColor = 2,

        /// <summary>
        /// 恢复为上一帧。
        /// </summary>
        RestoreToPrevious = 3
    }
}
