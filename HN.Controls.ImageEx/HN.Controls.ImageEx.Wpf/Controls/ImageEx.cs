using HN.Cache;
using HN.Pipes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    [TemplatePart(Name = ImageTemplateName, Type = typeof(Image))]
    [TemplatePart(Name = FailedContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = LoadingContentHostTemplateName, Type = typeof(ContentPresenter))]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = NormalStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = OpenedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = FailedStateName)]
    [TemplateVisualState(GroupName = ImageStateGroupName, Name = LoadingStateName)]
    public class ImageEx : Control
    {
        public static readonly DependencyProperty FailedTemplateProperty = DependencyProperty.Register(nameof(FailedTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty FailedTemplateSelectorProperty = DependencyProperty.Register(nameof(FailedTemplateSelector), typeof(DataTemplateSelector), typeof(ImageEx), new PropertyMetadata(default(DataTemplateSelector)));
        public static readonly DependencyProperty LoadingTemplateProperty = DependencyProperty.Register(nameof(LoadingTemplate), typeof(DataTemplate), typeof(ImageEx), new PropertyMetadata(default(DataTemplate)));
        public static readonly DependencyProperty LoadingTemplateSelectorProperty = DependencyProperty.Register(nameof(LoadingTemplateSelector), typeof(DataTemplateSelector), typeof(ImageEx), new PropertyMetadata(default(DataTemplateSelector)));
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageEx), new PropertyMetadata(default(object), OnSourceChanged));
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(nameof(StretchDirection), typeof(StretchDirection), typeof(ImageEx), new PropertyMetadata(StretchDirection.Both));
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform));

        private const string FailedContentHostTemplateName = "PART_FailedContentHost";
        private const string FailedStateName = "Failed";
        private const string ImageStateGroupName = "ImageStates";
        private const string ImageTemplateName = "PART_Image";
        private const string LoadingContentHostTemplateName = "PART_LoadingContentHost";
        private const string LoadingStateName = "Loading";
        private const string NormalStateName = "Normal";
        private const string OpenedStateName = "Opened";

        private static readonly Dictionary<Type, Func<object>> PipeServices = new Dictionary<Type, Func<object>>();
        private Image _image;
        private CancellationTokenSource _lastLoadCts;

        static ImageEx()
        {
            SetPipes(new[]
            {
                typeof(DirectPipe<ImageSource>),
                typeof(MemoryCachePipe<ImageSource>),
                typeof(StringPipe<ImageSource>),
                typeof(DiskCachePipe<ImageSource>),
                typeof(UriPipe<ImageSource>),
                typeof(ByteArrayPipe<ImageSource>),
                typeof(StreamToImageSourcePipe)
            });
            AddService<DiskCache, IDiskCache>(() => new DiskCache());
            AddService<HttpClientHandler, HttpMessageHandler>(() => new HttpClientHandler());

            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageEx), new FrameworkPropertyMetadata(typeof(ImageEx)));
        }

        public event EventHandler<ImageExFailedEventArgs> ImageFailed;

        public event EventHandler ImageOpened;

        public static IImmutableList<Type> Pipes { get; private set; }

        public DataTemplate FailedTemplate
        {
            get => (DataTemplate)GetValue(FailedTemplateProperty);
            set => SetValue(FailedTemplateProperty, value);
        }

        public DataTemplateSelector FailedTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(FailedTemplateSelectorProperty);
            set => SetValue(FailedTemplateSelectorProperty, value);
        }

        public DataTemplate LoadingTemplate
        {
            get => (DataTemplate)GetValue(LoadingTemplateProperty);
            set => SetValue(LoadingTemplateProperty, value);
        }

        public DataTemplateSelector LoadingTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(LoadingTemplateSelectorProperty);
            set => SetValue(LoadingTemplateSelectorProperty, value);
        }

        public object Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }

        public static void AddService<T, TInterface>(Func<T> serviceFactory) where T : TInterface
        {
            if (serviceFactory == null)
            {
                throw new ArgumentNullException(nameof(serviceFactory));
            }

            PipeServices[typeof(TInterface)] = () => serviceFactory();
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
                if (!typeof(PipeBase<ImageSource>).IsAssignableFrom(pipeType))
                {
                    throw new ArgumentException($"pipeType must inherit {nameof(PipeBase<ImageSource>)}");
                }
                pipeList.Add(pipeType);
            }
            Pipes = pipeList.ToImmutableList();
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = (Image)GetTemplateChild(ImageTemplateName);
            await SetSource(Source);
        }

        private static async void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;
            var value = e.NewValue;

            await obj.SetSource(value);
        }

        private async Task SetSource(object source)
        {
            if (_image == null)
            {
                return;
            }

            _lastLoadCts?.Cancel();
            if (source == null)
            {
                _image.Source = null;
                VisualStateManager.GoToState(this, NormalStateName, true);
            }
            else
            {
                _lastLoadCts = new CancellationTokenSource();
                try
                {
                    VisualStateManager.GoToState(this, LoadingStateName, true);

                    var context = new LoadingContext<ImageSource>()
                    {
                        OriginSource = source,
                        Current = source
                    };

                    var pipeDelegate = PipeBuilder.Build<ImageSource>(Pipes, PipeServices);
                    await pipeDelegate.Invoke(context, _lastLoadCts.Token);

                    if (!_lastLoadCts.IsCancellationRequested)
                    {
                        _image.Source = context.Result;
                        VisualStateManager.GoToState(this, OpenedStateName, true);
                        ImageOpened?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    if (!_lastLoadCts.IsCancellationRequested)
                    {
                        _image.Source = null;
                        VisualStateManager.GoToState(this, FailedStateName, true);
                        ImageFailed?.Invoke(this, new ImageExFailedEventArgs(source, ex));
                    }
                }
            }
        }
    }
}