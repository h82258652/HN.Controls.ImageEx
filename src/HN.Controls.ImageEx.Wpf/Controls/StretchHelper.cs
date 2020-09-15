using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HN.Controls
{
    internal static class StretchHelper
    {
        internal static Size CalculateScaleFactor(
            Size availableSize,
            Size contentSize,
            Stretch stretch,
            StretchDirection stretchDirection)
        {
            var scaleX = 1d;
            var scaleY = 1d;

            var isConstrainedWidth = !double.IsPositiveInfinity(availableSize.Width);
            var isConstrainedHeight = !double.IsPositiveInfinity(availableSize.Height);

            if ((stretch == Stretch.Uniform || stretch == Stretch.UniformToFill || stretch == Stretch.Fill) &&
                (isConstrainedWidth || isConstrainedHeight))
            {
                scaleX = Math.Abs(contentSize.Width) < double.Epsilon ? 0d : availableSize.Width / contentSize.Width;
                scaleY = Math.Abs(contentSize.Height) < double.Epsilon ? 0d : availableSize.Height / contentSize.Height;

                if (!isConstrainedWidth)
                {
                    scaleX = scaleY;
                }
                else if (!isConstrainedHeight)
                {
                    scaleY = scaleX;
                }
                else
                {
                    switch (stretch)
                    {
                        case Stretch.Uniform:
                            var minScale = Math.Min(scaleX, scaleY);
                            scaleX = minScale;
                            scaleY = minScale;
                            break;

                        case Stretch.UniformToFill:
                            var maxScale = Math.Max(scaleX, scaleY);
                            scaleX = maxScale;
                            scaleY = maxScale;
                            break;
                    }
                }

                switch (stretchDirection)
                {
                    case StretchDirection.UpOnly:
                        scaleX = Math.Max(scaleX, 1);
                        scaleY = Math.Max(scaleY, 1);
                        break;

                    case StretchDirection.DownOnly:
                        scaleX = Math.Min(scaleX, 1);
                        scaleY = Math.Min(scaleY, 1);
                        break;
                }
            }

            return new Size(scaleX, scaleY);
        }
    }
}
