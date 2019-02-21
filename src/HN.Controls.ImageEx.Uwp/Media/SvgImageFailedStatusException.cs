using System;
using Windows.UI.Xaml.Media.Imaging;

namespace HN.Media
{
    /// <inheritdoc />
    /// <summary>
    /// 表示 svg 图像加载失败的错误。
    /// </summary>
    public class SvgImageFailedStatusException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="SvgImageFailedStatusException" /> 类的新实例。
        /// </summary>
        /// <param name="status"> svg 图像加载的状态。</param>
        public SvgImageFailedStatusException(SvgImageSourceLoadStatus status)
        {
            if (!Enum.IsDefined(typeof(SvgImageSourceLoadStatus), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            Status = status;
        }

        /// <summary>
        /// 获取 svg 图像加载的状态。
        /// </summary>
        /// <returns>
        /// svg 图像加载的状态。
        /// </returns>
        public SvgImageSourceLoadStatus Status { get; }
    }
}
