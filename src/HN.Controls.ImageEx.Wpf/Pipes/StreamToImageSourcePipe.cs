using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using HN.Services;

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
        public override async Task InvokeAsync(ILoadingContext<ImageSource> context, LoadingPipeDelegate<ImageSource> next, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context.Current is Stream stream)
            {
                var tcs = new TaskCompletionSource<ImageSource>();
                await Task.Run(() =>
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                        bitmap.Freeze();
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
    }
}
