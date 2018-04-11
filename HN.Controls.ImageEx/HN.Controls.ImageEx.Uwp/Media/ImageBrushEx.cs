using HN.Cache;
using HN.Pipes;
using Polly;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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

        private static IImmutableList<Type> _pipes;
        private CancellationTokenSource _lastLoadCts;

        static ImageBrushEx()
        {
            SetPipes(new[]
            {
                typeof(DirectPipe<ICompositionSurface>),
                typeof(MemoryCachePipe<ICompositionSurface>),
                typeof(StringPipe<ICompositionSurface>),
                typeof(DiskCachePipe<ICompositionSurface>),
                typeof(UriPipe<ICompositionSurface>),
                typeof(ByteArrayPipe<ICompositionSurface>),
                typeof(StreamToCompositionSurfacePipe)
            });
            AddService<DiskCache, IDiskCache>(() => new DiskCache());
            AddService<HttpClientHandler, HttpMessageHandler>(() => new HttpClientHandler());
        }

        public event EventHandler<ImageBrushExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

        public static IEnumerable<Type> Pipes => _pipes;

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

        public static void AddService<T, TInterface>(Func<T> serviceFactory) where T : TInterface
        {
            PipeBuilder.AddService<T, TInterface>(serviceFactory);
        }

        public static void SetPipes(IEnumerable<Type> pipes)
        {
            if (pipes == null)
            {
                throw new ArgumentNullException(nameof(pipes));
            }

            var pipeList = new List<Type>();
            foreach (var pipeType in pipes)
            {
                if (!typeof(PipeBase<ICompositionSurface>).IsAssignableFrom(pipeType))
                {
                    throw new ArgumentException($"pipeType must inherit {nameof(PipeBase<ICompositionSurface>)}");
                }
                pipeList.Add(pipeType);
            }
            _pipes = pipeList.ToImmutableList();
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
                var context = new LoadingContext<ICompositionSurface>(source);

                var pipeDelegate = PipeBuilder.Build<ICompositionSurface>(Pipes);
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