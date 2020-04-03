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
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see cref="Stream" /> 类型，则该管道会转换为 <see cref="ICompositionSurface" /> 类型。
    /// </summary>
    public class StreamToCompositionSurfacePipe : LoadingPipeBase<ICompositionSurface>
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="StreamToCompositionSurfacePipe" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public StreamToCompositionSurfacePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<ICompositionSurface> context, LoadingPipeDelegate<ICompositionSurface> next, CancellationToken cancellationToken = default)
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
