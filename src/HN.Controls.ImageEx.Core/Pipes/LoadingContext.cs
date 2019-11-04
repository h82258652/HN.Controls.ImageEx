using System;
using System.Threading;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 加载上下文。
    /// </summary>
    /// <typeparam name="TResult">加载目标的类型。</typeparam>
    public class LoadingContext<TResult> : ILoadingContext<TResult> where TResult : class
    {
        private byte[]? _httpResponseBytes;
        private TResult _result;

        /// <summary>
        /// 初始化 <see cref="LoadingContext{TResult}" /> 类的新实例。
        /// </summary>
        /// <param name="uiContext">UI 线程上下文。</param>
        /// <param name="source">输入的源。</param>
        /// <param name="desiredWidth">需求的宽度。</param>
        /// <param name="desiredHeight">需求的高度。</param>
        public LoadingContext([NotNull]SynchronizationContext uiContext, [NotNull]object source, double? desiredWidth, double? desiredHeight)
        {
            UIContext = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            Current = OriginSource = source ?? throw new ArgumentNullException(nameof(source));
            DesiredWidth = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        /// <inheritdoc />
        public event EventHandler<HttpDownloadProgress> DownloadProgressChanged;

        /// <inheritdoc />
        public object Current { get; set; }

        /// <inheritdoc />
        public double? DesiredHeight { get; }

        /// <inheritdoc />
        public double? DesiredWidth { get; }

        /// <inheritdoc />
        public byte[]? HttpResponseBytes
        {
            get => _httpResponseBytes;
            set
            {
                if (_httpResponseBytes != null)
                {
                    throw new InvalidOperationException("value has been set.");
                }

                _httpResponseBytes = value;
            }
        }

        /// <inheritdoc />
        public object OriginSource { get; }

        /// <inheritdoc />
        public TResult? Result
        {
            get => _result;
            set
            {
                if (_result != null)
                {
                    throw new InvalidOperationException("value has been set.");
                }

                _result = value;
            }
        }

        /// <inheritdoc />
        public SynchronizationContext UIContext { get; }

        /// <inheritdoc />
        public void RaiseDownloadProgressChanged(HttpDownloadProgress progress)
        {
            DownloadProgressChanged?.Invoke(this, progress);
        }

        /// <summary>
        /// 重置加载上下文。
        /// </summary>
        public void Reset()
        {
            Current = OriginSource;
            Result = null;
            _httpResponseBytes = null;
        }
    }
}
