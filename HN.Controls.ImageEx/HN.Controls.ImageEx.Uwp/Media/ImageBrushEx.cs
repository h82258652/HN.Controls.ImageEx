using HN.Pipes;
using Polly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using HN.Services;

namespace HN.Media
{
    public class ImageBrushEx : XamlCompositionBrushBase
    {
        public static readonly DependencyProperty AlignmentXProperty = DependencyProperty.Register(nameof(AlignmentX), typeof(AlignmentX), typeof(ImageBrushEx), new PropertyMetadata(AlignmentX.Center, OnAlignmentXChanged));
        public static readonly DependencyProperty AlignmentYProperty = DependencyProperty.Register(nameof(AlignmentY), typeof(AlignmentY), typeof(ImageBrushEx), new PropertyMetadata(AlignmentY.Center, OnAlignmentYChanged));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(nameof(ImageSource), typeof(object), typeof(ImageBrushEx), new PropertyMetadata(default(object), OnImageSourceChanged));
        public static readonly DependencyProperty RetryCountProperty = DependencyProperty.Register(nameof(RetryCount), typeof(int), typeof(ImageBrushEx), new PropertyMetadata(default(int)));
        public static readonly DependencyProperty RetryDelayProperty = DependencyProperty.Register(nameof(RetryDelay), typeof(TimeSpan), typeof(ImageBrushEx), new PropertyMetadata(TimeSpan.Zero));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageBrushEx), new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

        private CancellationTokenSource _lastLoadCts;

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

        public AlignmentX AlignmentX
        {
            get => (AlignmentX)GetValue(AlignmentXProperty);
            set => SetValue(AlignmentXProperty, value);
        }

        public AlignmentY AlignmentY
        {
            get => (AlignmentY)GetValue(AlignmentYProperty);
            set => SetValue(AlignmentYProperty, value);
        }

        public object ImageSource
        {
            get => GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public int RetryCount
        {
            get => (int)GetValue(RetryCountProperty);
            set => SetValue(RetryCountProperty, value);
        }

        public TimeSpan RetryDelay
        {
            get => (TimeSpan)GetValue(RetryDelayProperty);
            set => SetValue(RetryDelayProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        protected override async void OnConnected()
        {
            base.OnConnected();

            await SetSourceAsync(ImageSource);
        }

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

        private async Task SetSourceAsync(object source)
        {
            _lastLoadCts?.Cancel();
            if (source == null)
            {
                CompositionBrush = null;
                return;
            }

            _lastLoadCts = new CancellationTokenSource();
            try
            {
                var context = new LoadingContext<ICompositionSurface>(source, null, null);

                var pipeDelegate = ImageExService.GetHandler<ICompositionSurface>();
                var retryDelay = RetryDelay;
                var policy = Policy.Handle<Exception>().WaitAndRetryAsync(RetryCount, count => retryDelay, (ex, delay) =>
                {
                    context.Reset();
                });
                await policy.ExecuteAsync(() => pipeDelegate.Invoke(context, _lastLoadCts.Token));

                if (!_lastLoadCts.IsCancellationRequested)
                {
                    DisposeCompositionBrush();
                    var compositor = Window.Current.Compositor;
                    CompositionBrush = compositor.CreateSurfaceBrush(context.Result);
                    ImageOpened?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!_lastLoadCts.IsCancellationRequested)
                {
                    DisposeCompositionBrush();
                    ImageFailed?.Invoke(this, new ImageBrushExFailedEventArgs(source, ex));
                }
            }
        }
    }
}