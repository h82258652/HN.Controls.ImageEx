using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Media;
using HN.Services;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace HN.Pipes
{
    public class StreamToCompositionSurfacePipe : PipeBase<ICompositionSurface>
    {
        public StreamToCompositionSurfacePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        public override async Task InvokeAsync(ILoadingContext<ICompositionSurface> context, PipeDelegate<ICompositionSurface> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is Stream stream)
            {
                var tcs = new TaskCompletionSource<LoadedImageSurface>();
                var imageSurface = LoadedImageSurface.StartLoadFromStream(stream.AsRandomAccessStream());

                TypedEventHandler<LoadedImageSurface, LoadedImageSourceLoadCompletedEventArgs> handler = null;
                handler = (sender, args) =>
                {
                    imageSurface.LoadCompleted -= handler;
                    switch (args.Status)
                    {
                        case LoadedImageSourceLoadStatus.Success:
                            tcs.SetResult(sender);
                            break;

                        default:
                            tcs.SetException(new ImageSurfaceFailedStatusException(args.Status));
                            break;
                    }
                };

                imageSurface.LoadCompleted += handler;
                context.Current = await tcs.Task;
            }

            await next(context, cancellationToken);
        }
    }
}