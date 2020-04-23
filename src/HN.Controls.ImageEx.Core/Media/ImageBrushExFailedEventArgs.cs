using System;
using JetBrains.Annotations;

namespace HN.Media
{
    /// <inheritdoc />
    /// <summary>
    /// 图像源加载失败事件数据。
    /// </summary>
    public class ImageBrushExFailedEventArgs : EventArgs
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageBrushExFailedEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="source">引起该加载失败的源。</param>
        /// <param name="failedException">说明加载失败原因的异常。</param>
        public ImageBrushExFailedEventArgs(object source, Exception failedException)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Exception = failedException ?? throw new ArgumentNullException(nameof(failedException));
        }

        /// <summary>
        /// 获取说明该加载失败原因的异常。
        /// </summary>
        /// <returns>
        /// 说明该加载失败原因的异常。
        /// </returns>
        [NotNull]
        public Exception Exception { get; }

        /// <summary>
        /// 获取引起该加载失败的源。
        /// </summary>
        /// <returns>
        /// 引起加载失败的源。
        /// </returns>
        [NotNull]
        public object Source { get; }
    }
}
