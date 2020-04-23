using System;
using System.Threading;
using HN.Http;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <summary>
    /// 加载上下文。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public interface ILoadingContext<in TSource> where TSource : class
    {
        /// <summary>
        /// 下载进度发生变化事件。
        /// </summary>
        [CanBeNull]
        event EventHandler<HttpDownloadProgress>? DownloadProgressChanged;

        /// <summary>
        /// 设置或获取当前管道的值。
        /// </summary>
        /// <returns>
        /// 当前管道的值。
        /// </returns>
        [NotNull]
        object Current { get; set; }

        /// <summary>
        /// 获取需求的高度。
        /// </summary>
        /// <returns>
        /// 需求的高度。
        /// </returns>
        double? DesiredHeight { get; }

        /// <summary>
        /// 获取需求的宽度。
        /// </summary>
        /// <returns>
        /// 需求的宽度。
        /// </returns>
        double? DesiredWidth { get; }

        /// <summary>
        /// 若管道处理过程中涉及 HTTP 传输，则存放 HTTP 响应的内容在此。
        /// </summary>
        /// <returns>
        /// 若管道处理过程中涉及 HTTP 传输，则有值。
        /// </returns>
        [CanBeNull]
        byte[]? HttpResponseBytes { get; set; }

        /// <summary>
        /// 获取原始输入的数据源。
        /// </summary>
        /// <returns>
        /// 原始输入的数据源。
        /// </returns>
        [NotNull]
        object OriginSource { get; }

        /// <summary>
        /// 获取 UI 线程上下文。
        /// </summary>
        /// <returns>
        /// UI 线程上下文。
        /// </returns>
        [NotNull]
        SynchronizationContext UIContext { get; }

        /// <summary>
        /// 附加目标源到可视树上。
        /// </summary>
        /// <param name="source">目标源对象。</param>
        void AttachSource([NotNull] TSource source);

        /// <summary>
        /// 发起下载进度发生变化事件。
        /// </summary>
        /// <param name="progress">当前下载进度。</param>
        void RaiseDownloadProgressChanged(HttpDownloadProgress progress);

        /// <summary>
        /// 重置该上下文为初始状态。
        /// </summary>
        void Reset();
    }
}
