using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Pipes;
using HN.Services;
using Polly;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace HN.Media
{
    /// <inheritdoc />
    /// <summary>
    /// 使用图像绘制区域。
    /// </summary>
    public class ImageBrushEx : XamlCompositionBrushBase
    {
        /// <summary>
        /// 标识 <see cref="AlignmentX" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="AlignmentX" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register(nameof(AlignmentX), typeof(AlignmentX), typeof(ImageBrushEx), new PropertyMetadata(AlignmentX.Center, OnAlignmentXChanged));

        /// <summary>
        /// 标识 <see cref="AlignmentY" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="AlignmentY" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.Register(nameof(AlignmentY), typeof(AlignmentY), typeof(ImageBrushEx), new PropertyMetadata(AlignmentY.Center, OnAlignmentYChanged));

        /// <summary>
        /// 标识 <see cref="ImageSource" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="ImageSource" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(object), typeof(ImageBrushEx), new PropertyMetadata(default(object), OnImageSourceChanged));

        /// <summary>
        /// 标识 <see cref="RetryCount" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="RetryCount" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty RetryCountProperty = DependencyProperty.Register(nameof(RetryCount), typeof(int), typeof(ImageBrushEx), new PropertyMetadata(default(int)));

        /// <summary>
        /// 标识 <see cref="RetryDelay" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="RetryDelay" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty RetryDelayProperty = DependencyProperty.Register(nameof(RetryDelay), typeof(TimeSpan), typeof(ImageBrushEx), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 标识 <see cref="Stretch" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Stretch" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageBrushEx), new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

        private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;
        private CancellationTokenSource _lastLoadCts;
        private object _lastLoadSource;

        /// <summary>
        /// 在无法加载图像源时发生。
        /// </summary>
        public event ImageBrushExFailedEventHandler ImageFailed;

        /// <summary>
        /// 在成功显示图像后发生。
        /// </summary>
        public event EventHandler ImageOpened;

        /// <summary>
        /// 获取或设置 <see cref="TileBrush" /> 基本磁贴中内容的水平对齐方式。
        /// </summary>
        /// <returns>
        /// 用于指定 <see cref="TileBrush" /> 内容在其基本磁贴中水平位置的值。
        /// 默认值为 <see cref="Windows.UI.Xaml.Media.AlignmentX.Center" />。
        /// </returns>
        public AlignmentX AlignmentX
        {
            get => (AlignmentX)GetValue(AlignmentXProperty);
            set => SetValue(AlignmentXProperty, value);
        }

        /// <summary>
        /// 获取或设置 <see cref="TileBrush" /> 基本磁贴中内容的垂直对齐方式。
        /// </summary>
        /// <returns>
        /// 用于指定 <see cref="TileBrush" /> 内容在其基本磁贴中垂直位置的值。
        /// 默认值为 <see cref="Windows.UI.Xaml.Media.AlignmentY.Center" />。
        /// </returns>
        public AlignmentY AlignmentY
        {
            get => (AlignmentY)GetValue(AlignmentYProperty);
            set => SetValue(AlignmentYProperty, value);
        }

        /// <summary>
        /// 获取图像真实显示的源。
        /// </summary>
        public ICompositionSurface HostSource => (CompositionBrush as CompositionSurfaceBrush)?.Surface;

        /// <summary>
        /// 获取或设置此 <see cref="ImageBrush" /> 显示的图像。
        /// </summary>
        /// <returns>
        /// 此 <see cref="ImageBrush" /> 显示的图像。
        /// </returns>
        public object ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// 获取或设置加载失败时的重试次数。
        /// </summary>
        /// <returns>
        /// 加载失败时的重试次数。
        /// </returns>
        public int RetryCount
        {
            get => (int)GetValue(RetryCountProperty);
            set => SetValue(RetryCountProperty, value);
        }

        /// <summary>
        /// 获取或设置加载失败时的重试间隔。
        /// </summary>
        /// <returns>
        /// 加载失败时的重试间隔。
        /// </returns>
        public TimeSpan RetryDelay
        {
            get => (TimeSpan)GetValue(RetryDelayProperty);
            set => SetValue(RetryDelayProperty, value);
        }

        /// <summary>
        /// 获取或设置一个值，它指定此 <see cref="TileBrush" /> 的内容如何拉伸才适合其磁贴。
        /// </summary>
        /// <returns>
        /// 指定此 <see cref="TileBrush" /> 内容如何投影到其基本磁贴的值。
        /// 默认值为 <see cref="Windows.UI.Xaml.Media.Stretch.Fill" />。
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <inheritdoc />
        protected override async void OnConnected()
        {
            base.OnConnected();

            await SetSourceAsync(ImageSource);
        }

        /// <inheritdoc />
        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            DisposeCompositionBrush();
        }

        private static void OnAlignmentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = (AlignmentX)e.NewValue;

            if (!Enum.IsDefined(typeof(AlignmentX), value))
            {
                throw new ArgumentOutOfRangeException(nameof(AlignmentX));
            }

            var brush = (CompositionSurfaceBrush)obj.CompositionBrush;
            brush.HorizontalAlignmentRatio = (float)value * 0.5F;
        }

        private static void OnAlignmentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = (AlignmentY)e.NewValue;

            if (!Enum.IsDefined(typeof(AlignmentY), value))
            {
                throw new ArgumentOutOfRangeException(nameof(AlignmentY));
            }

            var brush = (CompositionSurfaceBrush)obj.CompositionBrush;
            brush.VerticalAlignmentRatio = (float)value * 0.5F;
        }

        private static async void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = e.NewValue;

            await obj.SetSourceAsync(value);
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushEx)d;
            var value = (Stretch)e.NewValue;

            if (!Enum.IsDefined(typeof(Stretch), value))
            {
                throw new ArgumentOutOfRangeException(nameof(Stretch));
            }

            var brush = (CompositionSurfaceBrush)obj.CompositionBrush;
            brush.Stretch = (CompositionStretch)value;
        }

        private void DisposeCompositionBrush()
        {
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        private void SetBrush(CompositionBrush brush)
        {
            DisposeCompositionBrush();
            CompositionBrush = brush;
        }

        private async Task SetSourceAsync(object source)
        {
            if (_lastLoadSource == source)
            {
                return;
            }
            _lastLoadSource = source;

            var sourceSetter = ImageExService.GetSourceSetter<IImageBrushExSourceSetter>();

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                sourceSetter.SetSource(SetBrush, null);
                return;
            }

            var loadCts = new CancellationTokenSource();
            _lastLoadCts = loadCts;
            try
            {
                var context = new LoadingContext<ICompositionSurface>(_uiContext, source, null, null);

                var pipeDelegate = ImageExService.GetHandler<ICompositionSurface>();
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(RetryCount, count => retryDelay, (ex, delay) =>
                {
                    context.Reset();
                });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, loadCts.Token));

                if (!loadCts.IsCancellationRequested)
                {
                    sourceSetter.SetSource(SetBrush, context.Result);
                    ImageOpened?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!loadCts.IsCancellationRequested)
                {
                    sourceSetter.SetSource(SetBrush, null);
                    ImageFailed?.Invoke(this, new ImageBrushExFailedEventArgs(source, ex));
                }
            }
        }
    }
}
