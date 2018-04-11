using System;
using System.IO;
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
                var tcs = new TaskCompletionSource<ImageSource>();
                await Task.Run(() =>
                {
                    try
                    {
                        var bb = new BitmapImage();
                        bb.BeginInit();
                        bb.StreamSource = stream;
                        bb.EndInit();
                        bb.Freeze();
                        tcs.SetResult(bb);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }, CancellationToken.None);
                context.Result = await tcs.Task;
            }
            else
            {
                await next(context, cancellationToken);
            }
        }
    }
}