using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HN.Services;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see cref="Stream" /> 类型，则该管道会转换为 <see cref="ImageSource" /> 类型。
    /// </summary>
    public class StreamToImageSourcePipe : LoadingPipeBase<ImageSource>
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="StreamToImageSourcePipe" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public StreamToImageSourcePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<ImageSource> context, LoadingPipeDelegate<ImageSource> next, CancellationToken cancellationToken = default)
        {
            if (context.Current is Stream stream)
            {
                if (!stream.CanSeek)
                {
                    var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    stream = memoryStream;
                }

                var isWebP = IsWebP(stream);

                var tcs = new TaskCompletionSource<ImageSource>();
                await Task.Run(() =>
                {
                    Image webPImage = null;
                    if (isWebP)
                    {
                        var webPFormat = new WebPFormat();
                        webPImage = webPFormat.Load(stream);
                    }

                    if (webPImage != null)
                    {
                        var webPMemoryStream = new MemoryStream();
                        webPImage.Save(webPMemoryStream, ImageFormat.Png);
                        stream = webPMemoryStream;
                    }

                    stream.Seek(0, SeekOrigin.Begin);

                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
                        context.UIContext.Send(state =>
                        {
                            context.AttachSource(bitmap);
                        }, null);
                        tcs.SetResult(bitmap);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }, cancellationToken);
                context.Current = await tcs.Task;
            }

            await next(context, cancellationToken);
        }

        private static bool IsWebP(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var b1 = stream.ReadByte();
            var b2 = stream.ReadByte();
            var b3 = stream.ReadByte();
            var b4 = stream.ReadByte();
            var isWebP = b1 == 'R' && b2 == 'I' && b3 == 'F' && b4 == 'F';
            stream.Seek(0, SeekOrigin.Begin);
            return isWebP;
        }
    }
}
