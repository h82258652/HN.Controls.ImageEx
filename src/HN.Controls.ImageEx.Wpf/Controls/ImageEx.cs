using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HN.Pipes;
using HN.Services;
using Polly;

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
        public static readonly DependencyProperty IsLoadingProperty;

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
        /// 标识 <see cref="StretchDirection" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="StretchDirection" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(nameof(StretchDirection), typeof(StretchDirection), typeof(ImageEx), new PropertyMetadata(StretchDirection.Both));

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
        private static readonly DependencyPropertyKey IsLoadingPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsLoading), typeof(bool), typeof(ImageEx), new PropertyMetadata(default(bool)));

        private Image _image;
        private bool _isInViewport;
        private CancellationTokenSource _lastLoadCts;
        private object _lazyLoadingSource;

        static ImageEx()
        {
            IsLoadingProperty = IsLoadingPropertyKey.DependencyProperty;

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEx), new FrameworkPropertyMetadata(typeof(ImageEx)));
        }

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageEx" /> 类的新实例。
        /// </summary>
        public ImageEx()
        {
            LayoutUpdated += ImageEx_LayoutUpdated;
        }

        /// <summary>
        /// 在无法加载图像源时发生。
        /// </summary>
        public event ImageExFailedEventHandler ImageFailed;

        /// <summary>
        /// 在成功显示图像后发生。
        /// </summary>
        public event EventHandler ImageOpened;

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
        /// 获取是否正在加载图像的源。
        /// </summary>
        /// <returns>
        /// 是否正在加载图像的源。
        /// </returns>
        public bool IsLoading
        {
            get => (bool)GetValue(IsLoadingProperty);
            private set => SetValue(IsLoadingPropertyKey, value);
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
        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        /// <summary>
        /// 获取或设置一个值，该值描述应如何拉伸 <see cref="ImageEx" /> 以填充目标矩形。
        /// </summary>
        /// <returns>
        /// <see cref="System.Windows.Media.Stretch" /> 值之一。
        /// 默认值为 <see cref="System.Windows.Media.Stretch.Uniform" />。
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// 获取或设置一个值，指示如何将图像进行缩放。
        /// </summary>
        /// <returns>
        /// <see cref="System.Windows.Controls.StretchDirection" /> 值之一。
        /// 默认值为 <see cref="System.Windows.Controls.StretchDirection.Both" />。
        /// </returns>
        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }

        /// <inheritdoc />
        public override async void OnApplyTemplate()
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

        private async void ImageEx_LayoutUpdated(object sender, EventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            FrameworkElement hostElement = this;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(hostElement) as FrameworkElement;
                if (parent == null)
                {
                    break;
                }

                if (parent is ScrollViewer)
                {
                    hostElement = parent;
                    break;
                }

                hostElement = parent;
            }

            if (!ReferenceEquals(hostElement, this))
            {
                var controlRect = TransformToVisual(hostElement)
                    .TransformBounds(new Rect(0, 0, ActualWidth, ActualHeight));
                var hostRect = new Rect(0, 0, hostElement.ActualWidth, hostElement.ActualHeight);

                if (controlRect.IntersectsWith(hostRect))
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

                var context = new LoadingContext<ImageSource>(source, ActualWidth, ActualHeight);

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
