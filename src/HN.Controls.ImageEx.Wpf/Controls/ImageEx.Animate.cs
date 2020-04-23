using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using HN.Extensions;
using HN.Utils;

namespace HN.Controls
{
    public partial class ImageEx
    {
        private static void DrawGifFrame(DrawingContext context, IReadOnlyList<BitmapFrame> bitmapFrames, IReadOnlyList<GifFrameMetadata> frameMetadatas, int index)
        {
            var bitmapFrame = bitmapFrames[index];
            var gifFrameMetadata = frameMetadatas[index];

            if (index <= 0)
            {
                context.DrawImage(bitmapFrame, new Rect(gifFrameMetadata.Left, gifFrameMetadata.Top, gifFrameMetadata.Width, gifFrameMetadata.Height));
                return;
            }

            var previousGifFrameMetadata = frameMetadatas[index - 1];
            if (previousGifFrameMetadata.Disposal == FrameDisposalMethod.None || previousGifFrameMetadata.Disposal == FrameDisposalMethod.DoNotDispose)
            {
                DrawGifFrame(context, bitmapFrames, frameMetadatas, index - 1);
                context.DrawImage(bitmapFrame, new Rect(gifFrameMetadata.Left, gifFrameMetadata.Top, gifFrameMetadata.Width, gifFrameMetadata.Height));
            }
            else if (previousGifFrameMetadata.Disposal == FrameDisposalMethod.RestoreToBackgroundColor)
            {
                DrawGifFrame(context, bitmapFrames, frameMetadatas, index - 1);
                context.DrawRectangle(Brushes.White, new Pen(), new Rect(previousGifFrameMetadata.Left, previousGifFrameMetadata.Top, previousGifFrameMetadata.Width, previousGifFrameMetadata.Height));
                context.DrawImage(bitmapFrame, new Rect(gifFrameMetadata.Left, gifFrameMetadata.Top, gifFrameMetadata.Width, gifFrameMetadata.Height));
            }
            else if (previousGifFrameMetadata.Disposal == FrameDisposalMethod.RestoreToPrevious)
            {
                var i = index - 1;
                for (i = i - 1; i >= 0; i--)
                {
                    if (frameMetadatas[i].Disposal != FrameDisposalMethod.RestoreToPrevious)
                    {
                        break;
                    }
                }
                DrawGifFrame(context, bitmapFrames, frameMetadatas, i);
                context.DrawImage(bitmapFrame, new Rect(gifFrameMetadata.Left, gifFrameMetadata.Top, gifFrameMetadata.Width, gifFrameMetadata.Height));
            }
        }

        private static GifBitmapDecoder? GetGifBitmapDecoder(ImageSource? source)
        {
            GifBitmapDecoder? decoder = null;
            if (source is BitmapFrame bitmapFrame)
            {
                decoder = bitmapFrame.Decoder as GifBitmapDecoder;
            }
            else if (source is BitmapImage bitmap)
            {
                if (bitmap.UriSource != null)
                {
                    decoder = BitmapDecoder.Create(bitmap.UriSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad) as GifBitmapDecoder;
                }
                else if (bitmap.StreamSource != null)
                {
                    bitmap.StreamSource.Seek(0, SeekOrigin.Begin);
                    decoder = BitmapDecoder.Create(bitmap.StreamSource, BitmapCreateOptions.None, BitmapCacheOption.OnLoad) as GifBitmapDecoder;
                }
            }

            return decoder;
        }

        private static GifFrameMetadata GetGifFrameMetadata(BitmapFrame bitmapFrame)
        {
            var gifFrameMetadata = new GifFrameMetadata();

            if (bitmapFrame.Metadata is BitmapMetadata bitmapMetadata)
            {
                var delay = bitmapMetadata.GetQueryOrDefault<ushort>("/grctlext/Delay");
                gifFrameMetadata.Delay = TimeSpan.FromMilliseconds(delay * 10);
                var disposal = bitmapMetadata.GetQueryOrDefault<byte>("/grctlext/Disposal");
                gifFrameMetadata.Disposal = (FrameDisposalMethod)disposal;
                gifFrameMetadata.Left = bitmapMetadata.GetQueryOrDefault<ushort>("/imgdesc/Left");
                gifFrameMetadata.Top = bitmapMetadata.GetQueryOrDefault<ushort>("/imgdesc/Top");
                gifFrameMetadata.Width = bitmapMetadata.GetQueryOrDefault<ushort>("/imgdesc/Width");
                gifFrameMetadata.Height = bitmapMetadata.GetQueryOrDefault<ushort>("/imgdesc/Height");
            }

            return gifFrameMetadata;
        }

        private void AnimateSource()
        {
            if (_image == null)
            {
                return;
            }

            var source = _image.Source;
            var decoder = GetGifBitmapDecoder(source);
            if (decoder != null)
            {
                var bitmapFrames = decoder.Frames;

                var storyboard = new Storyboard();
                var animation = new ObjectAnimationUsingKeyFrames
                {
                    RepeatBehavior = RepeatBehavior.Forever
                };

                var frameMetadatas = bitmapFrames.Select(GetGifFrameMetadata).ToList();

                var timeSpan = TimeSpan.Zero;

                var width = decoder.Metadata.GetQueryOrDefault<ushort>("/logscrdesc/Width");
                var height = decoder.Metadata.GetQueryOrDefault<ushort>("/logscrdesc/Height");

                for (var i = 0; i < bitmapFrames.Count; i++)
                {
                    var bitmapFrame = bitmapFrames[i];
                    var frameMetadata = frameMetadatas[i];

                    var discreteObjectKeyFrame = new DiscreteObjectKeyFrame
                    {
                        KeyTime = timeSpan
                    };

                    var drawingVisual = new DrawingVisual();
                    using (var drawingContext = drawingVisual.RenderOpen())
                    {
                        DrawGifFrame(drawingContext, bitmapFrames, frameMetadatas, i);
                    }

                    var renderTargetBitmap = new RenderTargetBitmap(width, height, bitmapFrame.DpiX, bitmapFrame.DpiY, PixelFormats.Pbgra32);
                    renderTargetBitmap.Render(drawingVisual);
                    discreteObjectKeyFrame.Value = renderTargetBitmap;
                    animation.KeyFrames.Add(discreteObjectKeyFrame);

                    timeSpan += frameMetadata.Delay;
                }

                animation.Duration = timeSpan;
                Storyboard.SetTarget(animation, _image);
                Storyboard.SetTargetProperty(animation, new PropertyPath(nameof(_image.Source)));
                storyboard.Children.Add(animation);
                storyboard.Begin();
            }
            else
            {
                _image.BeginAnimation(Image.SourceProperty, null);
                _image.Source = source;
            }
        }
    }
}
