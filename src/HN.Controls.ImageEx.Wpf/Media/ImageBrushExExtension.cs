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
    public class ImageBrushExExtension : MarkupExtension
    {
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(nameof(ImageSource), typeof(object), typeof(ImageBrushExExtension), new PropertyMetadata(default(object)));

        private static readonly List<ImageBrushExExtension> Instances = new List<ImageBrushExExtension>();
        private AlignmentX _alignmentX = AlignmentX.Center;
        private AlignmentY _alignmentY = AlignmentY.Center;
        private ImageBrush _brush;
        private object _imageSource;
        private CancellationTokenSource _lastLoadCts;
        private object _lastLoadSource;
        private Stretch _stretch = Stretch.Fill;
        private DependencyObject _targetObject;
        private TileMode _tileMode = TileMode.None;
        private Rect _viewbox = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewboxUnits = BrushMappingMode.RelativeToBoundingBox;
        private Rect _viewport = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewportUnits = BrushMappingMode.RelativeToBoundingBox;

        public ImageBrushExExtension()
        {
            Instances.Add(this);
        }

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

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

        public object ImageSource
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

        public ImageBrush Clone()
        {
            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush.Clone();
        }

        public ImageBrush CloneCurrentValue()
        {
            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush.CloneCurrentValue();
        }

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
                _brush.ImageSource = null;
                return;
            }

            _lastLoadCts = new CancellationTokenSource();
            try
            {
                var context = new LoadingContext<ImageSource>(source, null, null);

                var pipeDelegate = ImageExService.GetHandler<ImageSource>();
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(RetryCount, count => retryDelay, (ex, delay) =>
                {
                    context.Reset();
                });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, _lastLoadCts.Token));

                if (!_lastLoadCts.IsCancellationRequested)
                {
                    _brush.ImageSource = context.Result;
                    ImageOpened?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!_lastLoadCts.IsCancellationRequested)
                {
                    _brush.ImageSource = null;
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
