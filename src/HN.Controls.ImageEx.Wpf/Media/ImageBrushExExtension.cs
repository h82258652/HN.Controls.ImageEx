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
        public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.RegisterAttached(nameof(AlignmentX), typeof(AlignmentX), typeof(ImageBrushExExtension), new PropertyMetadata(AlignmentX.Center, OnAlignmentXChanged));
        public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.RegisterAttached(nameof(AlignmentY), typeof(AlignmentY), typeof(ImageBrushExExtension), new PropertyMetadata(AlignmentY.Center, OnAlignmentYChanged));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.RegisterAttached(nameof(ImageSource), typeof(object), typeof(ImageBrushExExtension), new PropertyMetadata(default(object), OnImageSourceChanged));
        public static readonly DependencyProperty RetryCountProperty = DependencyProperty.RegisterAttached(nameof(RetryCount), typeof(int), typeof(ImageBrushExExtension), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty RetryDelayProperty = DependencyProperty.RegisterAttached(nameof(RetryDelay), typeof(TimeSpan), typeof(ImageBrushExExtension), new PropertyMetadata(TimeSpan.Zero));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.RegisterAttached(nameof(Stretch), typeof(Stretch), typeof(ImageBrushExExtension), new PropertyMetadata(Stretch.Fill, OnStretchChanged));
        public static readonly DependencyProperty TileModeProperty = DependencyProperty.RegisterAttached(nameof(TileMode), typeof(TileMode), typeof(ImageBrushExExtension), new PropertyMetadata(TileMode.None, OnTileModeChanged));
        public static readonly DependencyProperty ViewboxProperty = DependencyProperty.RegisterAttached(nameof(Viewbox), typeof(Rect), typeof(ImageBrushExExtension), new PropertyMetadata(new Rect(0.0, 0.0, 1.0, 1.0), OnViewboxChanged));
        public static readonly DependencyProperty ViewboxUnitsProperty = DependencyProperty.RegisterAttached(nameof(ViewboxUnits), typeof(BrushMappingMode), typeof(ImageBrushExExtension), new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnViewboxUnitsChanged));
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.RegisterAttached(nameof(Viewport), typeof(Rect), typeof(ImageBrushExExtension), new PropertyMetadata(new Rect(0.0, 0.0, 1.0, 1.0), OnViewportChanged));
        public static readonly DependencyProperty ViewportUnitsProperty = DependencyProperty.RegisterAttached(nameof(ViewportUnits), typeof(BrushMappingMode), typeof(ImageBrushExExtension), new PropertyMetadata(BrushMappingMode.RelativeToBoundingBox, OnViewportUnitsChanged));
        private readonly ImageBrushExInternal _internal;
        private ImageBrush _brush;
        private CancellationTokenSource _lastLoadCts;

        public ImageBrushExExtension()
        {
            _internal = new ImageBrushExInternal(this);
        }

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

        public AlignmentX AlignmentX
        {
            get => (AlignmentX)_internal.GetValue(AlignmentXProperty);
            set => _internal.SetValue(AlignmentXProperty, value);
        }

        public AlignmentY AlignmentY
        {
            get => (AlignmentY)_internal.GetValue(AlignmentYProperty);
            set => _internal.SetValue(AlignmentYProperty, value);
        }

        public object ImageSource
        {
            get => _internal.GetValue(ImageSourceProperty);
            set => _internal.SetValue(ImageSourceProperty, value);
        }

        public int RetryCount
        {
            get => (int)_internal.GetValue(RetryCountProperty);
            set => _internal.SetValue(RetryCountProperty, value);
        }

        public TimeSpan RetryDelay
        {
            get => (TimeSpan)_internal.GetValue(RetryDelayProperty);
            set => _internal.SetValue(RetryDelayProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)_internal.GetValue(StretchProperty);
            set => _internal.SetValue(StretchProperty, value);
        }

        public TileMode TileMode
        {
            get => (TileMode)_internal.GetValue(TileModeProperty);
            set => _internal.SetValue(TileModeProperty, value);
        }

        public Rect Viewbox
        {
            get => (Rect)_internal.GetValue(ViewboxProperty);
            set => _internal.SetValue(ViewboxProperty, value);
        }

        public BrushMappingMode ViewboxUnits
        {
            get => (BrushMappingMode)_internal.GetValue(ViewboxUnitsProperty);
            set => _internal.SetValue(ViewboxUnitsProperty, value);
        }

        public Rect Viewport
        {
            get => (Rect)_internal.GetValue(ViewportProperty);
            set => _internal.SetValue(ViewportProperty, value);
        }

        public BrushMappingMode ViewportUnits
        {
            get => (BrushMappingMode)_internal.GetValue(ViewportUnitsProperty);
            set => _internal.SetValue(ViewportUnitsProperty, value);
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

        private static void OnAlignmentXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (AlignmentX)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.AlignmentX = value;
            }
        }

        private static void OnAlignmentYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (AlignmentY)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.AlignmentY = value;
            }
        }

        private static async void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = e.NewValue;

            if (obj.Owner._brush != null)
            {
                await obj.Owner.SetSourceAsync(value);
            }
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (Stretch)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.Stretch = value;
            }
        }

        private static void OnTileModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (TileMode)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.TileMode = value;
            }
        }

        private static void OnViewboxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (Rect)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.Viewbox = value;
            }
        }

        private static void OnViewboxUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (BrushMappingMode)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.ViewboxUnits = value;
            }
        }

        private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (Rect)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.Viewport = value;
            }
        }

        private static void OnViewportUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageBrushExInternal)d;
            var value = (BrushMappingMode)e.NewValue;

            if (obj.Owner._brush != null)
            {
                obj.Owner._brush.ViewportUnits = value;
            }
        }

        private async void CreateImageBrush()
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
            await SetSourceAsync(ImageSource);
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
