using System;
using Windows.UI.Xaml.Media;

namespace HN.Media
{
    /// <inheritdoc />
    /// <summary>
    /// 表示图像加载失败的错误。
    /// </summary>
    public class ImageSurfaceFailedStatusException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageSurfaceFailedStatusException" /> 类的新实例。
        /// </summary>
        /// <param name="status">图像加载的状态。</param>
        public ImageSurfaceFailedStatusException(LoadedImageSourceLoadStatus status)
        {
            if (!Enum.IsDefined(typeof(LoadedImageSourceLoadStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            Status = status;
        }

        /// <summary>
        /// 获取图像加载的状态。
        /// </summary>
        /// <returns>
        /// 图像加载的状态。
        /// </returns>
        public LoadedImageSourceLoadStatus Status { get; }
    }
}
