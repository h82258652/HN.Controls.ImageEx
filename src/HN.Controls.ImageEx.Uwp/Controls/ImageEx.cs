using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Http;
using HN.Models;
using HN.Pipes;
using HN.Services;
using Polly;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HN.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// 表示具备缓存功能的显示图像的控件。
    /// </summary>
    [TemplatePart(Name = CanvasTemplateName, Type = typeof(SKXamlCanvas))]
    [TemplatePart(Name = FailedContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = LoadingContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = EmptyStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = OpenedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = FailedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = LoadingStateName)]
    public partial class ImageEx : Control
    {
        /// <summary>
        /// 标识 <see cref="DownloadProgress" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="DownloadProgress" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty DownloadProgressProperty = DependencyProperty.Register(nameof(DownloadProgress), typeof(HttpDownloadProgress), typeof(ImageEx), new PropertyMetadata(default(HttpDownloadProgress), OnDownloadProgressChanged));

        /// <summary>
        /// 标识 <see cref="FailedTemplate" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="FailedTemplate" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty FailedTemplateProperty = DependencyProperty.Register(nameof(FailedTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// 标识 <see cref="FailedTemplateSelector" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="FailedTemplateSelector" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty FailedTemplateSelectorProperty = DependencyProperty.Register(nameof(FailedTemplateSelector), typeof(DataTemplateSelector), typeof(ImageEx), new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 标识 <see cref="IsLoading" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="IsLoading" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(ImageEx), new PropertyMetadata(default(bool)));

        /// <summary>
        /// 标识 <see cref="LoadingTemplate" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="LoadingTemplate" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty LoadingTemplateProperty = DependencyProperty.Register(nameof(LoadingTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// 标识 <see cref="LoadingTemplateSelector" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="LoadingTemplateSelector" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty LoadingTemplateSelectorProperty = DependencyProperty.Register(nameof(LoadingTemplateSelector), typeof(DataTemplateSelector), typeof(ImageEx), new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 标识 <see cref="PlaceholderTemplate" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="PlaceholderTemplate" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty PlaceholderTemplateProperty = DependencyProperty.Register(nameof(PlaceholderTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));

        /// <summary>
        /// 标识 <see cref="PlaceholderTemplateSelector" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="PlaceholderTemplateSelector" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty PlaceholderTemplateSelectorProperty = DependencyProperty.Register(nameof(PlaceholderTemplateSelector), typeof(DataTemplateSelector), typeof(ImageEx), new PropertyMetadata(default(DataTemplateSelector)));

        /// <summary>
        /// 标识 <see cref="Source" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Source" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageEx), new PropertyMetadata(default, OnSourceChanged));

        private const string CanvasTemplateName = "PART_Canvas";
        private const string EmptyStateName = "Empty";
        private const string FailedContentHostTemplateName = "PART_FailedContentHost";
        private const string FailedStateName = "Failed";
        private const string ImageStateGroupName = "ImageStates";
        private const string LoadingContentHostTemplateName = "PART_LoadingContentHost";
        private const string LoadingStateName = "Loading";
        private const string OpenedStateName = "Opened";
        private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;
        private SKXamlCanvas? _canvas;
        private IImageExDisplaySource? _displaySource;
        private CancellationTokenSource? _lastLoadCts;
        private ContentPresenter? _loadingContentHost;

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageEx" /> 类的新实例。
        /// </summary>
        public ImageEx()
        {
            if (_isInDesignMode)
            {
                DefaultStyleResourceUri = new Uri("ms-appx:///HN.Controls.ImageEx.Uwp/Controls/ImageEx.Design.xaml");
            }

            DefaultStyleKey = typeof(ImageEx);

            Loaded += ImageEx_Loaded;
            Unloaded += ImageEx_Unloaded;
            LayoutUpdated += ImageEx_LayoutUpdated;
        }

        /// <summary>
        /// 在下载进度发生变化时发生。
        /// </summary>
        public event EventHandler<HttpDownloadProgress>? DownloadProgressChanged;

        /// <summary>
        /// 在无法加载图像源时发生。
        /// </summary>
        public event ImageExFailedEventHandler? ImageFailed;

        /// <summary>
        /// 在成功显示图像后发生。
        /// </summary>
        public event EventHandler? ImageOpened;

        /// <summary>
        /// 获取当前图像的下载进度。
        /// </summary>
        /// <returns>
        /// 当前图像的下载进度。
        /// </returns>
        public HttpDownloadProgress DownloadProgress
        {
            get => (HttpDownloadProgress)GetValue(DownloadProgressProperty);
            private set => SetValue(DownloadProgressProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载失败时的内容的数据模板。
        /// </summary>
        /// <returns>
        /// 用于显示加载失败时的内容的数据模板。
        /// </returns>
        public DataTemplate? FailedTemplate
        {
            get => (DataTemplate?)GetValue(FailedTemplateProperty);
            set => SetValue(FailedTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载失败时的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示加载失败时的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector? FailedTemplateSelector
        {
            get => (DataTemplateSelector?)GetValue(FailedTemplateSelectorProperty);
            set => SetValue(FailedTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取是否正在加载图像的源。
        /// </summary>
        /// <returns>
        /// 是否正在加载图像的源。
        /// </returns>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            private set => SetValue(IsLoadingProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载中的内容的数据模板。
        /// </summary>
        /// <returns>
        /// 用于显示加载中的内容的数据模板。
        /// </returns>
        public DataTemplate? LoadingTemplate
        {
            get => (DataTemplate?)GetValue(LoadingTemplateProperty);
            set => SetValue(LoadingTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载中的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示加载中的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector? LoadingTemplateSelector
        {
            get => (DataTemplateSelector?)GetValue(LoadingTemplateSelectorProperty);
            set => SetValue(LoadingTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示占位的内容的数据模板。
        /// </summary>
        /// <returns>
        /// 用于显示占位的内容的数据模板。
        /// </returns>
        public DataTemplate? PlaceholderTemplate
        {
            get => (DataTemplate?)GetValue(PlaceholderTemplateProperty);
            set => SetValue(PlaceholderTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示占位的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示占位的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector? PlaceholderTemplateSelector
        {
            get => (DataTemplateSelector?)GetValue(PlaceholderTemplateSelectorProperty);
            set => SetValue(PlaceholderTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取或设置图像的源。
        /// </summary>
        /// <returns>
        /// 所绘制图像的源。
        /// </returns>
        public object? Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        internal void InvalidateCanvas()
        {
            _canvas?.Invalidate();
        }

        /// <inheritdoc />
        protected override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _root = (UIElement)GetTemplateChild(RootTemplateName);

            _shadowExpandBorder = (Border)GetTemplateChild(ShadowExpandBorderTemplateName);
            UpdateShadowExpandBorder();

            _canvas = (SKXamlCanvas)GetTemplateChild(CanvasTemplateName);
            if (_canvas != null)
            {
                _canvas.PaintSurface += Canvas_PaintSurface;
            }

            _image = (Image)GetTemplateChild(ImageTemplateName);

            _loadingContentHost = (ContentPresenter)GetTemplateChild(LoadingContentHostTemplateName);

            InvalidateMeasure();

            if (Source == null || !LazyLoadingEnabled || _isInViewport)
            {
                _lazyLoadingSource = null;
                await SetSourceAsync(Source);
            }
            else
            {
                _lazyLoadingSource = Source;
            }
        }

        private static void OnDownloadProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = (HttpDownloadProgress)e.NewValue;
            obj.DownloadProgressChanged?.Invoke(obj, value);
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = e.NewValue;

            if (obj.Source == null || !obj.LazyLoadingEnabled || obj._isInViewport)
            {
                obj._lazyLoadingSource = null;
                await obj.SetSourceAsync(value);
            }
            else
            {
                obj._lazyLoadingSource = value;
            }
        }

        private void AttachSource(ImageExSource? source)
        {
            var displaySource = BuildDisplaySource(source);
            if (displaySource != null)
            {
                if (displaySource is IMultiplyFrameImageExDisplaySource multiplyFrameDisplaySource)
                {
                    multiplyFrameDisplaySource.CurrentChanged += DisplaySource_CurrentChanged;
                    if (AutoStart)
                    {
                        multiplyFrameDisplaySource.Play();
                    }
                }
            }

            _displaySource = displaySource;

            InvalidateMeasure();
            InvalidateCanvas();
        }

        private IImageExDisplaySource? BuildDisplaySource(ImageExSource? source)
        {
            if (source == null)
            {
                return null;
            }

            if (source.Frames.Length == 1)
            {
                return new SingleFrameImageExDisplaySource(source.Frames[0].Bitmap, source.Width, source.Height);
            }
            else
            {
                return new MultiplyFrameImageExDisplaySource(source, RepeatBehavior, SpeedRatio);
            }
        }

        private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var info = e.Info;
            var surface = e.Surface;
            var canvas = surface.Canvas;

            canvas.Clear();

            var bitmap = _displaySource?.Current;
            if (bitmap == null)
            {
                return;
            }

            using var paint = new SKPaint
            {
                IsAntialias = true
            };

            var dpi = GetDpi();

            float x = (float)(_shadowXExpand * dpi);
            float y = (float)(_shadowYExpand * dpi);
            float width = info.Width - 2 * x;
            float height = info.Height - 2 * y;

            using var contentBitmap = new SKBitmap(info.Width, info.Height);
            using (var contentCanvas = new SKCanvas(contentBitmap))
            {
                var cornerRadius = CornerRadius;
                if (cornerRadius != default)
                {
                    using var cornerRadiusPath = BuildCornerRadiusPath(
                        x,
                        y,
                        width,
                        height,
                        (float)(cornerRadius.TopLeft * dpi),
                        (float)(cornerRadius.TopRight * dpi),
                        (float)(cornerRadius.BottomRight * dpi),
                        (float)(cornerRadius.BottomLeft * dpi));
                    contentCanvas.ClipPath(cornerRadiusPath, antialias: paint.IsAntialias);
                }

                var dest = new SKRect(x, y, x + width, y + height);
                DrawBitmapWithStretch(contentCanvas, bitmap, dest, (float)dpi, paint);

                contentCanvas.Flush();
            }

            SetPaintShadow(paint);

            canvas.DrawBitmap(contentBitmap, info.Rect, paint);
        }

        private void DisplaySource_CurrentChanged(object sender, SKBitmap e)
        {
            InvalidateCanvas();
        }

        private double GetDpi()
        {
            return _canvas!.Dpi;
        }

        private void ImageEx_LayoutUpdated(object sender, object e)
        {
            InvalidateLazyLoading();
        }

        private void ImageEx_Loaded(object sender, RoutedEventArgs e)
        {
            RegisterCornerRadiusChanged();
        }

        private void ImageEx_Unloaded(object sender, RoutedEventArgs e)
        {
            UnregisterCornerRadiusChanged();
        }

        private async Task SetRuntimeSourceAsync(object? source)
        {
            if (_canvas == null)
            {
                return;
            }

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                AttachSource(null);
                VisualStateManager.GoToState(this, EmptyStateName, true);
                return;
            }

            var loadCts = new CancellationTokenSource();
            _lastLoadCts = loadCts;
            try
            {
                IsLoading = true;

                VisualStateManager.GoToState(this, LoadingStateName, true);

                // 开始 Loading，重置 DownloadProgress
                DownloadProgress = default;

                var context = new LoadingContext<ImageExSource>(_uiContext, source, AttachSource, ActualWidth, ActualHeight);
                context.DownloadProgressChanged += (sender, progress) =>
                {
                    _uiContext.Post(state =>
                    {
                        DownloadProgress = progress;
                    }, null);
                };

                var pipeDelegate = ImageExService.GetHandler<ImageExSource>();
                var retryCount = RetryCount;
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(retryCount, count => retryDelay, (ex, delay) =>
                    {
                        context.Reset();
                    });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, loadCts.Token));

                if (!loadCts.IsCancellationRequested)
                {
                    VisualStateManager.GoToState(this, OpenedStateName, true);
                    PlayFadeInAnimation();
                    _uiContext.Post(state =>
                    {
                        ImageOpened?.Invoke(this, EventArgs.Empty);
                    }, null);
                }
            }
            catch (Exception ex)
            {
                if (!loadCts.IsCancellationRequested)
                {
                    AttachSource(null);
                    VisualStateManager.GoToState(this, FailedStateName, true);
                    _uiContext.Post(state =>
                    {
                        ImageFailed?.Invoke(this, new ImageExFailedEventArgs(source, ex));
                    }, null);
                }
            }
            finally
            {
                if (!loadCts.IsCancellationRequested)
                {
                    IsLoading = false;
                }
            }
        }

        private Task SetSourceAsync(object? source)
        {
            if (_isInDesignMode)
            {
                return SetDesignSourceAsync(source);
            }
            else
            {
                return SetRuntimeSourceAsync(source);
            }
        }
    }
}
