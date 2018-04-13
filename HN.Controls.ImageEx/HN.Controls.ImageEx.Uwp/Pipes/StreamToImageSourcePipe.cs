using HN.Media;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HN.Pipes
{
    public class StreamToImageSourcePipe : PipeBase<ImageSource>
    {
        public override async Task InvokeAsync(LoadingContext<ImageSource> context, PipeDelegate<ImageSource> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is Stream stream)
            {
                var isStartWithLessThanSign = false;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    isStartWithLessThanSign = stream.ReadByte() == '<'; // svg start with <
                    stream.Seek(0, SeekOrigin.Begin);
                }

                if (isStartWithLessThanSign)
                {
                    var bitmap = new SvgImageSource();
                    var svgImageLoadStatus = await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                    if (svgImageLoadStatus != SvgImageSourceLoadStatus.Success)
                    {
                        throw new SvgImageFailedStatusException(svgImageLoadStatus);
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    context.Current = bitmap;
                }
                else
                {
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                    cancellationToken.ThrowIfCancellationRequested();
                    context.Current = bitmap;
                }
            }
            await next(context, cancellationToken);
        }
    }
}