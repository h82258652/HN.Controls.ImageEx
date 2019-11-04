using System;
using System.Threading;
using System.Threading.Tasks;
using HN.Pipes;
using HN.Services;
using Polly;
using Windows.Media.Casting;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HN.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// 表示具备缓存功能的显示图像的控件。
    /// </summary>
    [TemplatePart(Name = ImageTemplateName, Type = typeof(Image))]
    [TemplatePart(Name = FailedContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = LoadingContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = EmptyStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = OpenedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = FailedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = LoadingStateName)]
    public class ImageEx : Control
    {
        /// <summary>
        /// 标识 <see cref="DownloadProgress" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="DownloadProgress" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty DownloadProgressProperty = DependencyProperty.Register(nameof(DownloadProgress), typeof(HttpDownloadProgress), typeof(ImageEx), new PropertyMetadata(default(HttpDownloadProgress)));

        /// <summary>
        /// 标识 <see cref="EnableLazyLoading" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="EnableLazyLoading" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty EnableLazyLoadingProperty = DependencyProperty.Register(nameof(EnableLazyLoading), typeof(bool), typeof(ImageEx), new PropertyMetadata(default(bool)));

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
        /// 标识 <see cref="NineGrid" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="NineGrid" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty NineGridProperty = DependencyProperty.Register(nameof(NineGrid), typeof(Thickness), typeof(ImageEx), new PropertyMetadata(default(Thickness)));

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
        /// 标识 <see cref="RetryCount" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="RetryCount" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty RetryCountProperty = DependencyProperty.Register(nameof(RetryCount), typeof(int), typeof(ImageEx), new PropertyMetadata(default(int)));

        /// <summary>
        /// 标识 <see cref="RetryDelay" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="RetryDelay" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty RetryDelayProperty = DependencyProperty.Register(nameof(RetryDelay), typeof(TimeSpan), typeof(ImageEx), new PropertyMetadata(TimeSpan.Zero));

        /// <summary>
        /// 标识 <see cref="Source" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Source" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageEx), new PropertyMetadata(default(object), OnSourceChanged));

        /// <summary>
        /// 标识 <see cref="Stretch" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Stretch" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform));

        private const string EmptyStateName = "Empty";
        private const string FailedContentHostTemplateName = "PART_FailedContentHost";
        private const string FailedStateName = "Failed";
        private const string ImageStateGroupName = "ImageStates";
        private const string ImageTemplateName = "PART_Image";
        private const string LoadingContentHostTemplateName = "PART_LoadingContentHost";
        private const string LoadingStateName = "Loading";
        private const string OpenedStateName = "Opened";

        private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;
        private Image _image;
        private bool _isInViewport;
        private CancellationTokenSource _lastLoadCts;
        private object _lazyLoadingSource;

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageEx" /> 类的新实例。
        /// </summary>
        public ImageEx()
        {
            DefaultStyleKey = typeof(ImageEx);

            EffectiveViewportChanged += ImageEx_EffectiveViewportChanged;
        }

        /// <summary>
        /// 在下载进度发生变化时发生。
        /// </summary>
        public event EventHandler<HttpDownloadProgress> DownloadProgressChanged;

        /// <summary>
        /// 在无法加载图像源时发生。
        /// </summary>
        public event ImageExFailedEventHandler ImageFailed;

        /// <summary>
        /// 在成功显示图像后发生。
        /// </summary>
        public event EventHandler ImageOpened;

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
        /// 获取或设置是否启用延迟加载。
        /// </summary>
        /// <returns>
        /// 是否启用延迟加载。
        /// </returns>
        public bool EnableLazyLoading
        {
            get => (bool)GetValue(EnableLazyLoadingProperty);
            set => SetValue(EnableLazyLoadingProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载失败时的内容的数据模板。
        /// </summary>
        /// <returns>
        /// 用于显示加载失败时的内容的数据模板。
        /// </returns>
        public DataTemplate FailedTemplate
        {
            get => (DataTemplate)GetValue(FailedTemplateProperty);
            set => SetValue(FailedTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载失败时的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示加载失败时的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector FailedTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(FailedTemplateSelectorProperty);
            set => SetValue(FailedTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取图像真实显示的源。
        /// </summary>
        public ImageSource HostSource => _image?.Source;

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
        public DataTemplate LoadingTemplate
        {
            get => (DataTemplate)GetValue(LoadingTemplateProperty);
            set => SetValue(LoadingTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示加载中的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示加载中的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector LoadingTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(LoadingTemplateSelectorProperty);
            set => SetValue(LoadingTemplateSelectorProperty, value);
        }

        /// <summary>
        /// 获取或设置控制图像小大调整方式的九格形式的值。九网格形式使你可以将图像的边缘和角拉伸成与其中心不同的形状。
        /// </summary>
        /// <returns>
        /// 为九网格大小调整比喻设置 **Left**、**Top**、**Right**、**Bottom** 量化指标的 Thickness 值。
        /// </returns>
        public Thickness NineGrid
        {
            get => (Thickness)GetValue(NineGridProperty);
            set => SetValue(NineGridProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示占位的内容的数据模板。
        /// </summary>
        /// <returns>
        /// 用于显示占位的内容的数据模板。
        /// </returns>
        public DataTemplate PlaceholderTemplate
        {
            get => (DataTemplate)GetValue(PlaceholderTemplateProperty);
            set => SetValue(PlaceholderTemplateProperty, value);
        }

        /// <summary>
        /// 获取或设置用于显示占位的内容的数据模板选择器。
        /// </summary>
        /// <returns>
        /// 用于显示占位的内容的数据模板选择器。
        /// </returns>
        public DataTemplateSelector PlaceholderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(PlaceholderTemplateSelectorProperty);
            set => SetValue(PlaceholderTemplateSelectorProperty, value);
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

        /// <summary>
        /// 获取或设置一个值，该值描述应如何拉伸 <see cref="ImageEx" /> 以填充目标矩形。
        /// </summary>
        /// <returns>
        /// <see cref="Windows.UI.Xaml.Media.Stretch" /> 值之一。
        /// 默认值为 <see cref="Windows.UI.Xaml.Media.Stretch.Uniform" />。
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// 返回将图像的 alpha 通道表示为 <see cref="CompositionBrush" /> 的掩码。
        /// </summary>
        /// <returns>
        /// 表示图像的 alpha 通道的掩码。
        /// </returns>
        public CompositionBrush GetAlphaMask()
        {
            ApplyTemplate();
            return _image?.GetAlphaMask();
        }

        /// <summary>
        /// 返回图像作为 <see cref="CastingSource" />。
        /// </summary>
        /// <returns>
        /// 图像作为 <see cref="CastingSource" />。
        /// </returns>
        public CastingSource GetAsCastingSource()
        {
            ApplyTemplate();
            return _image?.GetAsCastingSource();
        }

        /// <inheritdoc />
        protected override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = (Image)GetTemplateChild(ImageTemplateName);
            if (Source == null || !EnableLazyLoading || _isInViewport)
            {
                _lazyLoadingSource = null;
                await SetSourceAsync(Source);
            }
            else
            {
                _lazyLoadingSource = Source;
            }
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = e.NewValue;

            if (obj.Source == null || !obj.EnableLazyLoading || obj._isInViewport)
            {
                obj._lazyLoadingSource = null;
                await obj.SetSourceAsync(value);
            }
            else
            {
                obj._lazyLoadingSource = value;
            }
        }

        private async void ImageEx_EffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
        {
            var bringIntoViewDistanceX = args.BringIntoViewDistanceX;
            var bringIntoViewDistanceY = args.BringIntoViewDistanceY;

            var width = ActualWidth;
            var height = ActualHeight;

            if (bringIntoViewDistanceX <= width && bringIntoViewDistanceY <= height)
            {
                _isInViewport = true;

                if (_lazyLoadingSource != null)
                {
                    var source = _lazyLoadingSource;
                    _lazyLoadingSource = null;
                    await SetSourceAsync(source);
                }
            }
            else
            {
                _isInViewport = false;
            }
        }

        private async Task SetSourceAsync(object source)
        {
            if (_image == null)
            {
                return;
            }

            var sourceSetter = ImageExService.GetSourceSetter<IImageExSourceSetter>();

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                sourceSetter.SetSource(_image, null);
                VisualStateManager.GoToState(this, EmptyStateName, true);
                return;
            }

            var loadCts = new CancellationTokenSource();
            _lastLoadCts = loadCts;
            try
            {
                IsLoading = true;

                VisualStateManager.GoToState(this, LoadingStateName, true);

                var context = new LoadingContext<ImageSource>(_uiContext, source, ActualWidth, ActualHeight);
                context.DownloadProgressChanged += (sender, progress) =>
                {
                    _uiContext.Post(state =>
                    {
                        DownloadProgress = progress;
                        DownloadProgressChanged?.Invoke(this, progress);
                    }, null);
                };

                var pipeDelegate = ImageExService.GetHandler<ImageSource>();
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(RetryCount, count => retryDelay, (ex, delay) =>
                    {
                        context.Reset();
                    });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, loadCts.Token));

                if (!loadCts.IsCancellationRequested)
                {
                    sourceSetter.SetSource(_image, context.Result);
                    VisualStateManager.GoToState(this, OpenedStateName, true);
                    ImageOpened?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!loadCts.IsCancellationRequested)
                {
                    sourceSetter.SetSource(_image, null);
                    VisualStateManager.GoToState(this, FailedStateName, true);
                    ImageFailed?.Invoke(this, new ImageExFailedEventArgs(source, ex));
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
    }
}
