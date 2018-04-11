using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HN.Pipes
{
    public class StreamToImageSourcePipe : PipeBase<ImageSource>
    {
        public override async Task InvokeAsync(LoadingContext<ImageSource> context, PipeDelegate<ImageSource> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is Stream stream)
            {
                var bitmap = await Task.Run(() =>
                {
                    var asyncBitmap = new BitmapImage();
                    asyncBitmap.BeginInit();
                    asyncBitmap.StreamSource = stream;
                    asyncBitmap.EndInit();
                    asyncBitmap.Freeze();
                    return asyncBitmap;
                }, CancellationToken.None);
                context.Result = bitmap;
            }
            else
            {
                await next(context, cancellationToken);
            }
        }
    }
}