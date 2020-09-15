using System;
using SkiaSharp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HN.Controls
{
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="StretchDirection" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="StretchDirection" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register(nameof(StretchDirection), typeof(StretchDirection), typeof(ImageEx), new PropertyMetadata(StretchDirection.Both, OnStretchDirectionChanged));

        /// <summary>
        /// 标识 <see cref="Stretch" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="Stretch" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageEx), new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

        /// <summary>
        /// 获取或设置一个值，该值描述应如何拉伸 <see cref="ImageEx" /> 以填充目标矩形。
        /// </summary>
        /// <returns>
        /// <see cref="Windows.UI.Xaml.Media.Stretch" /> 值之一。
        /// 默认值为 <see cref="Windows.UI.Xaml.Media.Stretch.Uniform" />。
        /// </returns>
        public Stretch Stretch
        {
            get => (Stretch)GetValue(StretchProperty);
            set => SetValue(StretchProperty, value);
        }

        /// <summary>
        /// 获取或设置一个值，指示如何将图像进行缩放。
        /// </summary>
        /// <returns>
        /// <see cref="Windows.UI.Xaml.Controls.StretchDirection" /> 值之一。
        /// 默认值为 <see cref="Windows.UI.Xaml.Controls.StretchDirection.Both" />。
        /// </returns>
        public StretchDirection StretchDirection
        {
            get => (StretchDirection)GetValue(StretchDirectionProperty);
            set => SetValue(StretchDirectionProperty, value);
        }

        private static SKRect CalculateDisplayRect(SKRect dest, float bitmapWidth, float bitmapHeight)
        {
            var x = dest.Left + (dest.Width - bitmapWidth) / 2;
            var y = dest.Top + (dest.Height - bitmapHeight) / 2;
            return new SKRect(x, y, x + bitmapWidth, y + bitmapHeight);
        }

        private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;

            obj.InvalidateCanvas();
        }

        private static void OnStretchDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;

            obj.InvalidateCanvas();
        }

        private void DrawBitmapWithStretch(SKCanvas canvas, SKBitmap bitmap, SKRect dest, float dpi, SKPaint? paint = null)
        {
            if (canvas == null)
            {
                throw new ArgumentNullException(nameof(canvas));
            }

            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            var stretch = Stretch;
            var stretchDirection = StretchDirection;
            if (stretch == Stretch.Fill)
            {
                if (stretchDirection == StretchDirection.DownOnly)
                {
                    var bitmapWidth = Math.Min(bitmap.Width * dpi, dest.Width);
                    var bitmapHeight = Math.Min(bitmap.Height * dpi, dest.Height);
                    var display = CalculateDisplayRect(dest, bitmapWidth, bitmapHeight);
                    canvas.DrawBitmap(bitmap, display, paint);
                }
                else if (stretchDirection == StretchDirection.UpOnly)
                {
                    var bitmapWidth = Math.Max(bitmap.Width * dpi, dest.Width);
                    var bitmapHeight = Math.Max(bitmap.Height * dpi, dest.Height);
                    var display = CalculateDisplayRect(dest, bitmapWidth, bitmapHeight);
                    canvas.DrawBitmap(bitmap, display, paint);
                }
                else
                {
                    canvas.DrawBitmap(bitmap, dest, paint);
                }
            }
            else
            {
                float scale = dpi;

                switch (stretch)
                {
                    case Stretch.None:
                        scale = dpi;
                        break;

                    case Stretch.Uniform:
                        {
                            var newScale = Math.Min(dest.Width / bitmap.Width, dest.Height / bitmap.Height);

                            if (stretchDirection == StretchDirection.Both)
                            {
                                scale = newScale;
                            }
                            else if (stretchDirection == StretchDirection.DownOnly)
                            {
                                scale = Math.Min(scale, newScale);
                            }
                            else if (stretchDirection == StretchDirection.UpOnly)
                            {
                                scale = Math.Max(scale, newScale);
                            }

                            break;
                        }

                    case Stretch.UniformToFill:
                        {
                            var newScale = Math.Max(dest.Width / bitmap.Width, dest.Height / bitmap.Height);

                            if (stretchDirection == StretchDirection.Both)
                            {
                                scale = newScale;
                            }
                            else if (stretchDirection == StretchDirection.DownOnly)
                            {
                                scale = Math.Min(scale, newScale);
                            }
                            else if (stretchDirection == StretchDirection.UpOnly)
                            {
                                scale = Math.Max(scale, newScale);
                            }

                            break;
                        }
                }

                var display = CalculateDisplayRect(dest, scale * bitmap.Width, scale * bitmap.Height);

                if (NineGrid != default)
                {
                    DrawBitmapWithNineGrid(canvas, bitmap, display, paint);
                }
                else
                {
                    canvas.DrawBitmap(bitmap, display, paint);
                }
            }
        }
    }
}
