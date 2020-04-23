using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using HN.Pipes;
using HN.Services;
using Polly;

namespace HN.Media
{
    /// <inheritdoc />
    /// <summary>
    /// 使用图像绘制区域。
    /// </summary>
    public class ImageBrushExExtension : MarkupExtension
    {
        /// <summary>
        /// 标识 <see cref="ImageSource" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="ImageSource" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(nameof(ImageSource), typeof(object), typeof(ImageBrushExExtension), new PropertyMetadata(default(object)));

        private static readonly List<ImageBrushExExtension> Instances = new List<ImageBrushExExtension>();
        private readonly SynchronizationContext _uiContext = SynchronizationContext.Current;
        private AlignmentX _alignmentX = AlignmentX.Center;
        private AlignmentY _alignmentY = AlignmentY.Center;
        private ImageBrush? _brush;
        private object? _imageSource;
        private CancellationTokenSource _lastLoadCts;
        private object _lastLoadSource;
        private Stretch _stretch = Stretch.Fill;
        private DependencyObject _targetObject;
        private TileMode _tileMode = TileMode.None;
        private Rect _viewbox = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewboxUnits = BrushMappingMode.RelativeToBoundingBox;
        private Rect _viewport = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewportUnits = BrushMappingMode.RelativeToBoundingBox;

        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="ImageBrushExExtension" /> 类的新实例。
        /// </summary>
        public ImageBrushExExtension()
        {
            Instances.Add(this);
        }

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
        /// 默认值为 <see cref="System.Windows.Media.AlignmentX.Center" />。
        /// </returns>
        public AlignmentX AlignmentX
        {
            get => _alignmentX;
            set
            {
                if (_alignmentX != value)
                {
                    _alignmentX = value;
                    if (_brush != null)
                    {
                        _brush.AlignmentX = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="TileBrush" /> 基本磁贴中内容的垂直对齐方式。
        /// </summary>
        /// <returns>
        /// 用于指定 <see cref="TileBrush" /> 内容在其基本磁贴中垂直位置的值。
        /// 默认值为 <see cref="System.Windows.Media.AlignmentY.Center" />。
        /// </returns>
        public AlignmentY AlignmentY
        {
            get => _alignmentY;
            set
            {
                if (_alignmentY != value)
                {
                    _alignmentY = value;
                    if (_brush != null)
                    {
                        _brush.AlignmentY = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取图像真实显示的源。
        /// </summary>
        public ImageSource? HostSource => _brush?.ImageSource;

        /// <summary>
        /// 获取或设置此 <see cref="ImageBrush" /> 显示的图像。
        /// </summary>
        /// <returns>
        /// 此 <see cref="ImageBrush" /> 显示的图像。
        /// </returns>
        public object? ImageSource
        {
            get => _imageSource;
            set
            {
                if (_imageSource != value)
                {
                    if (_imageSource is Binding && _targetObject != null)
                    {
                        // old value is Binding
                        BindingOperations.ClearBinding(_targetObject, ImageSourceBindingHelper.ImageSourceBindingProperty);
                    }

                    _imageSource = value;
                    if (value is Binding newValueBinding)
                    {
                        if (_targetObject != null)
                        {
                            BindingOperations.SetBinding(_targetObject, ImageSourceBindingHelper.ImageSourceBindingProperty, newValueBinding);
                        }
                    }
                    else
                    {
                        if (_brush != null)
                        {
                            SetSource(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置加载失败时的重试次数。
        /// </summary>
        /// <returns>
        /// 加载失败时的重试次数。
        /// </returns>
        public int RetryCount { get; set; }

        /// <summary>
        /// 获取或设置加载失败时的重试间隔。
        /// </summary>
        /// <returns>
        /// 加载失败时的重试间隔。
        /// </returns>
        public TimeSpan RetryDelay { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 获取或设置一个值，它指定此 <see cref="TileBrush" /> 的内容如何拉伸才适合其磁贴。
        /// </summary>
        /// <returns>
        /// 指定此 <see cref="TileBrush" /> 内容如何投影到其基本磁贴的值。
        /// 默认值为 <see cref="System.Windows.Media.Stretch.Fill" />。
        /// </returns>
        public Stretch Stretch
        {
            get => _stretch;
            set
            {
                if (_stretch != value)
                {
                    _stretch = value;
                    if (_brush != null)
                    {
                        _brush.Stretch = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指定在基本图块小于输出区时 <see cref="TileBrush" /> 如何填充你正在绘画的区域。
        /// </summary>
        /// <returns>
        /// 一个值，该值指定在 <see cref="Viewport" /> 属性指定的基本图块小于输出区域时 <see cref="TileBrush" /> 图块如何填充输出区域的值。
        /// 默认值为 <see cref="System.Windows.Media.TileMode.None" />。
        /// </returns>
        public TileMode TileMode
        {
            get => _tileMode;
            set
            {
                if (_tileMode != value)
                {
                    _tileMode = value;
                    if (_brush != null)
                    {
                        _brush.TileMode = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="TileBrush" /> 图块中内容的位置和尺寸。
        /// </summary>
        /// <returns>
        /// <see cref="TileBrush" /> 内容的位置和尺寸。
        /// 默认值为矩形 (<see cref="Rect" />)，其 <see cref="Rect.TopLeft" /> 为 (0,0)，<see cref="Rect.Width" /> 和 <see cref="Rect.Height" /> 为 1。
        /// </returns>
        public Rect Viewbox
        {
            get => _viewbox;
            set
            {
                if (_viewbox != value)
                {
                    _viewbox = value;
                    if (_brush != null)
                    {
                        _brush.Viewbox = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指定 <see cref="Viewbox" /> 值是相对于 <see cref="TileBrush" /> 内容的边界框而言，还是绝对值。
        /// </summary>
        /// <returns>
        /// 一个值，该值指示 <see cref="Viewbox" /> 值是相对于 <see cref="TileBrush" /> 内容的边界框而言，还是绝对值。
        /// 默认值为 <see cref="BrushMappingMode.RelativeToBoundingBox" />。
        /// </returns>
        public BrushMappingMode ViewboxUnits
        {
            get => _viewboxUnits;
            set
            {
                if (_viewboxUnits != value)
                {
                    _viewboxUnits = value;
                    if (_brush != null)
                    {
                        _brush.ViewboxUnits = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="TileBrush" /> 的基本图块的位置和尺寸。
        /// </summary>
        /// <returns>
        /// <see cref="TileBrush" /> 的基本图块的位置和尺寸。
        /// 默认值是一个矩形 (<see cref="Rect" />)，其 <see cref="Rect.TopLeft" /> 为 (0,0)，其 <see cref="Rect.Width" /> 和 <see cref="Rect.Height" /> 为 1。
        /// </returns>
        public Rect Viewport
        {
            get => _viewport;
            set
            {
                if (_viewport != value)
                {
                    _viewport = value;
                    if (_brush != null)
                    {
                        _brush.Viewport = value;
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置 <see cref="BrushMappingMode" /> 枚举，该枚举指定 <see cref="Viewport" /> 的值（该值指示 <see cref="TileBrush" /> 基本图块的大小和位置）是否是相对于输出区域的大小。
        /// </summary>
        /// <returns>
        /// 指示 <see cref="Viewport" /> 的值（该值用于描述 <see cref="TileBrush" /> 磁贴的大小和位置）是否是相对于整体输出区域的大小。
        /// 默认值为 <see cref="BrushMappingMode.RelativeToBoundingBox "/>。
        /// </returns>
        public BrushMappingMode ViewportUnits
        {
            get => _viewportUnits;
            set
            {
                if (_viewportUnits != value)
                {
                    _viewportUnits = value;
                    if (_brush != null)
                    {
                        _brush.ViewportUnits = value;
                    }
                }
            }
        }

        /// <summary>
        /// 创建的可修改复本 <see cref="ImageBrush" />，从而深度复制此对象的值。
        /// </summary>
        /// <returns>
        /// 当前对象的可修改复本。
        /// 即使源的 <see cref="Freezable.IsFrozen" /> 属性为 <see langword="true" />，所克隆对象的 <see cref="Freezable.IsFrozen" /> 属性也为 <see langword="false" />。
        /// </returns>
        public ImageBrush Clone()
        {
            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush.Clone();
        }

        /// <summary>
        /// 创建此 <see cref="ImageBrush" /> 对象的可修改复本，从而深度复制此对象的当前值。
        /// </summary>
        /// <returns>
        /// 当前对象的可修改复本。
        /// 克隆的对象 <see cref="Freezable.IsFrozen" /> 属性是 <see langword="false" /> 即使源的 <see cref="Freezable.IsFrozen" /> 属性是 <see langword="true" />。
        /// </returns>
        public ImageBrush CloneCurrentValue()
        {
            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush.CloneCurrentValue();
        }

        /// <inheritdoc />
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget)
            {
                _targetObject = provideValueTarget.TargetObject as DependencyObject;
                if (_targetObject != null && ImageSource is Binding binding)
                {
                    BindingOperations.SetBinding(_targetObject, ImageSourceBindingHelper.ImageSourceBindingProperty, binding);
                }
            }

            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush;
        }

        private void AttachSource(ImageSource source)
        {
            _brush.ImageSource = source;
        }

        private void CreateImageBrush()
        {
            _brush = new ImageBrush
            {
                ViewportUnits = ViewportUnits,
                ViewboxUnits = ViewboxUnits,
                Viewport = Viewport,
                Viewbox = Viewbox,
                Stretch = Stretch,
                TileMode = TileMode,
                AlignmentX = AlignmentX,
                AlignmentY = AlignmentY
            };
            if (!(ImageSource is Binding))
            {
                SetSource(ImageSource);
            }
        }

        private async void SetSource(object source)
        {
            await SetSourceAsync(source);
        }

        private async Task SetSourceAsync(object source)
        {
            if (_lastLoadSource == source)
            {
                return;
            }
            _lastLoadSource = source;

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                AttachSource(null);
                return;
            }

            var loadCts = new CancellationTokenSource();
            _lastLoadCts = loadCts;
            try
            {
                var context = new LoadingContext<ImageSource>(_uiContext, source, AttachSource, null, null);

                var pipeDelegate = ImageExService.GetHandler<ImageSource>();
                var retryCount = RetryCount;
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(retryCount, count => retryDelay, (ex, delay) =>
                {
                    context.Reset();
                });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, loadCts.Token));

                if (!loadCts.IsCancellationRequested)
                {
                    ImageOpened?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!loadCts.IsCancellationRequested)
                {
                    AttachSource(null);
                    ImageFailed?.Invoke(this, new ImageBrushExFailedEventArgs(source, ex));
                }
            }
        }

        private static class ImageSourceBindingHelper
        {
            internal static readonly DependencyProperty ImageSourceBindingProperty = DependencyProperty.RegisterAttached("ImageSourceBinding", typeof(object), typeof(DependencyObject), new PropertyMetadata(default(object), OnImageSourceBindingChagned));

            private static void OnImageSourceBindingChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var binding = BindingOperations.GetBinding(d, e.Property);
                foreach (var imageBrushExExtension in Instances.Where(temp => temp.ImageSource == binding))
                {
                    imageBrushExExtension.SetSource(e.NewValue);
                }
            }
        }
    }
}
