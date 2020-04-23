using System;
using System.Threading;
using HN.Http;
using JetBrains.Annotations;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 加载上下文。
    /// </summary>
    /// <typeparam name="TSource">加载源目标的类型。</typeparam>
    public class LoadingContext<TSource> : ILoadingContext<TSource> where TSource : class
    {
        private readonly Action<TSource>? _attachSource;
        private object _current;
        private byte[]? _httpResponseBytes;

        /// <summary>
        /// 初始化 <see cref="LoadingContext{TSource}" /> 类的新实例。
        /// </summary>
        /// <param name="uiContext">UI 线程上下文。</param>
        /// <param name="source">输入的源。</param>
        /// <param name="attachSource"></param>
        /// <param name="desiredWidth">需求的宽度。</param>
        /// <param name="desiredHeight">需求的高度。</param>
        public LoadingContext(
            [NotNull] SynchronizationContext uiContext,
            [NotNull]object source,
            [CanBeNull] Action<TSource>? attachSource,
            double? desiredWidth,
            double? desiredHeight)
        {
            UIContext = uiContext ?? throw new ArgumentNullException(nameof(uiContext));
            OriginSource = source ?? throw new ArgumentNullException(nameof(source));
            _current = source;
            _attachSource = attachSource;
            DesiredWidth = desiredWidth;
            DesiredHeight = desiredHeight;
        }

        /// <inheritdoc />
        public event EventHandler<HttpDownloadProgress>? DownloadProgressChanged;

        /// <inheritdoc />
        public object Current
        {
            get => _current;
            set => _current = value ?? throw new InvalidOperationException("can't set Current to null.");
        }

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
        public SynchronizationContext UIContext { get; }

        /// <inheritdoc />
        public void AttachSource(TSource source)
        {
            _attachSource?.Invoke(source);
        }

        /// <inheritdoc />
        public void RaiseDownloadProgressChanged(HttpDownloadProgress progress)
        {
            DownloadProgressChanged?.Invoke(this, progress);
        }

        /// <inheritdoc />
        public virtual void Reset()
        {
            Current = OriginSource;
            _httpResponseBytes = null;
        }
    }
}
