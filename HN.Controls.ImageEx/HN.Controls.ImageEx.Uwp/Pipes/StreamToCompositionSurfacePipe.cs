using HN.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace HN.Pipes
{
    public class StreamToCompositionSurfacePipe : PipeBase<ICompositionSurface>
    {
        public override async Task InvokeAsync(LoadingContext<ICompositionSurface> context, PipeDelegate<ICompositionSurface> next, CancellationToken cancellationToken = default(CancellationToken))
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
                context.Result = await tcs.Task;
            }
            else
            {
                await next(context, cancellationToken);
            }
        }
    }
}