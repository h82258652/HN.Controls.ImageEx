using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using HN.Pipes;
using HN.Services;
using Polly;

namespace HN.Media
{
    public class ImageBrushExExtension : MarkupExtension
    {
        private AlignmentX _alignmentX = AlignmentX.Center;
        private AlignmentY _alignmentY = AlignmentY.Center;
        private ImageBrush _brush;
        private object _imageSource;
        private CancellationTokenSource _lastLoadCts;
        private Stretch _stretch = Stretch.Fill;
        private TileMode _tileMode = TileMode.None;
        private Rect _viewbox = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewboxUnits = BrushMappingMode.RelativeToBoundingBox;
        private Rect _viewport = new Rect(0.0, 0.0, 1.0, 1.0);
        private BrushMappingMode _viewportUnits = BrushMappingMode.RelativeToBoundingBox;

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

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
                    _imageSource = value;
                    if (_brush != null)
                    {
                        SetSource(value);
                    }
                }
            }
        }

        public int RetryCount { get; set; }

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
            if (_brush == null)
            {
                CreateImageBrush();
            }

            return _brush;
        }

        private async void CreateImageBrush()
        {
            _brush = new ImageBrush()
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
            await SetSourceAsync(ImageSource);
        }

        private async void SetSource(object source)
        {
            await SetSourceAsync(source);
        }

        private async Task SetSourceAsync(object source)
        {
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
    }
}
