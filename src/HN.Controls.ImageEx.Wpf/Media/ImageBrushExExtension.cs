using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using HN.Pipes;
using HN.Services;
using JetBrains.Annotations;
using Polly;

namespace HN.Media
{
    public class ImageBrushExExtension : MarkupExtension
    {
        [NotNull] private readonly ImageBrush _brush = new ImageBrush
        {
            AlignmentX = AlignmentX.Center,
            AlignmentY = AlignmentY.Center,
            Viewbox = new Rect(0, 0, 1, 1),
            ViewboxUnits = BrushMappingMode.RelativeToBoundingBox,
            Viewport = new Rect(0, 0, 1, 1),
            ViewportUnits = BrushMappingMode.RelativeToBoundingBox,
        };

        private object _imageSource;
        private CancellationTokenSource _lastLoadCts;

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

        public AlignmentX AlignmentX
        {
            get => _brush.AlignmentX;
            set => _brush.AlignmentX = value;
        }

        public AlignmentY AlignmentY
        {
            get => _brush.AlignmentY;
            set => _brush.AlignmentY = value;
        }

        public object ImageSource
        {
            get => _imageSource;
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;
                    SetSource(value);
                }
            }
        }

        public int RetryCount { get; set; }

        public TimeSpan RetryDelay { get; set; } = TimeSpan.Zero;

        public Stretch Stretch
        {
            get => _brush.Stretch;
            set => _brush.Stretch = value;
        }

        public TileMode TileMode
        {
            get => _brush.TileMode;
            set => _brush.TileMode = value;
        }

        public Rect Viewbox
        {
            get => _brush.Viewbox;
            set => _brush.Viewbox = value;
        }

        public BrushMappingMode ViewboxUnits
        {
            get => _brush.ViewboxUnits;
            set => _brush.ViewboxUnits = value;
        }

        public Rect Viewport
        {
            get => _brush.Viewport;
            set => _brush.Viewport = value;
        }

        public BrushMappingMode ViewportUnits
        {
            get => _brush.ViewportUnits;
            set => _brush.ViewportUnits = value;
        }

        public ImageBrush Clone()
        {
            return _brush.Clone();
        }

        public ImageBrush CloneCurrentValue()
        {
            return _brush.CloneCurrentValue();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _brush;
        }

        private async void SetSource([CanBeNull] object source)
        {
            await SetSourceAsync(source);
        }

        private async Task SetSourceAsync([CanBeNull] object source)
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
