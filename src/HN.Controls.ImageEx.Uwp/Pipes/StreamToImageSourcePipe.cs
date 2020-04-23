using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Media;
using HN.Services;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

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

                var isSvg = IsSvg(stream);

                var tcs = new TaskCompletionSource<object>();
                context.UIContext.Post(async state =>
                {
                    try
                    {
                        if (isSvg)
                        {
                            var bitmap = new SvgImageSource();
                            context.AttachSource(bitmap);
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
                            context.AttachSource(bitmap);
                            await bitmap.SetSourceAsync(stream.AsRandomAccessStream());
                            cancellationToken.ThrowIfCancellationRequested();
                            context.Current = bitmap;
                        }
                        tcs.SetResult(null);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                }, null);
                await tcs.Task;
            }

            await next(context, cancellationToken);
        }

        private static bool IsSvg(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var isSvg = stream.ReadByte() == '<';
            stream.Seek(0, SeekOrigin.Begin);
            return isSvg;
        }
    }
}
