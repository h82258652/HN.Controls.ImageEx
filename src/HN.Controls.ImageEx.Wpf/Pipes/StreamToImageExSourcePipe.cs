﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using HN.Models;
using HN.Services;
using SkiaSharp;

namespace HN.Pipes
{
    /// <inheritdoc />
    /// <summary>
    /// 若当前的值是 <see cref="Stream" /> 类型，则该管道会转换为 <see cref="ImageExSource" /> 类型。
    /// </summary>
    public class StreamToImageExSourcePipe : LoadingPipeBase<ImageExSource>
    {
        /// <inheritdoc />
        /// <summary>
        /// 初始化 <see cref="StreamToImageExSourcePipe" /> 类的新实例。
        /// </summary>
        /// <param name="designModeService">设计模式服务。</param>
        public StreamToImageExSourcePipe(IDesignModeService designModeService) : base(designModeService)
        {
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(ILoadingContext<ImageExSource> context, LoadingPipeDelegate<ImageExSource> next, CancellationToken cancellationToken = default)
        {
            if (context.Current is Stream stream)
            {
                if (!stream.CanSeek)
                {
                    var memoryStream = new MemoryStream();
#if NETCOREAPP3_1
                    await stream.CopyToAsync(memoryStream, cancellationToken);
                    if (!ReferenceEquals(stream, context.OriginSource))
                    {
                        // if the stream generated by the pipe then dispose it.
                        await stream.DisposeAsync();
                    }
#else
                    await stream.CopyToAsync(memoryStream);
                    if (!ReferenceEquals(stream, context.OriginSource))
                    {
                        // if the stream generated by the pipe then dispose it.
                        stream.Dispose();
                    }
#endif
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    stream = memoryStream;
                }

                var tcs = new TaskCompletionSource<ImageExSource>();
                await Task.Run(() =>
                {
                    using var codec = SKCodec.Create(stream, out var codecResult);
                    if (codecResult != SKCodecResult.Success)
                    {
                        tcs.SetException(new Exception(codecResult.ToString()));
                        return;
                    }
                    
                    var codecInfo = codec.Info;

                    var frameCount = codec.FrameCount;
                    if (frameCount <= 0)
                    {
                        var source = new ImageExSource
                        {
                            Frames = new ImageExFrame[1],
                            RepetitionCount = codec.RepetitionCount,
                            Width = codecInfo.Width,
                            Height = codecInfo.Height
                        };

                        var bitmap = new SKBitmap(codecInfo);
                        var pointer = bitmap.GetPixels();
                        codec.GetPixels(bitmap.Info, pointer);

                        source.Frames[0] = new ImageExFrame
                        {
                            Bitmap = bitmap
                        };

                        context.InvokeOnUIThread(() =>
                        {
                            context.AttachSource(source);
                        });

                        tcs.SetResult(source);
                    }
                    else
                    {
                        var source = new ImageExSource
                        {
                            Frames = new ImageExFrame[frameCount],
                            RepetitionCount = codec.RepetitionCount,
                            Width = codecInfo.Width,
                            Height = codecInfo.Height
                        };

                        for (var frameIndex = 0; frameIndex < frameCount; frameIndex++)
                        {
                            var bitmap = new SKBitmap(codecInfo);
                            var pointer = bitmap.GetPixels();
                            var codecOptions = new SKCodecOptions(frameIndex);
                            codec.GetPixels(bitmap.Info, pointer, codecOptions);

                            var frameInfo = codec.FrameInfo[frameIndex];
                            var duration = frameInfo.Duration;

                            source.Frames[frameIndex] = new ImageExFrame
                            {
                                Bitmap = bitmap,
                                Duration = duration
                            };
                        }

                        context.InvokeOnUIThread(() =>
                        {
                            context.AttachSource(source);
                        });

                        tcs.SetResult(source);
                    }
                }, cancellationToken);
                context.Current = await tcs.Task;

#if NETCOREAPP3_1
                if (!ReferenceEquals(stream, context.OriginSource))
                {
                    // if the stream generated by the pipe then dispose it.
                    await stream.DisposeAsync();
                }
#else
                if (!ReferenceEquals(stream, context.OriginSource))
                {
                    // if the stream generated by the pipe then dispose it.
                    stream.Dispose();
                }
#endif
            }

            await next(context, cancellationToken);
        }
    }
}
