using System;
using System.Collections.Generic;
using System.Windows;
using SkiaSharp;

namespace HN.Controls
{
    public partial class ImageEx
    {
        /// <summary>
        /// 标识 <see cref="CornerRadius" /> 依赖属性。
        /// </summary>
        /// <returns>
        /// <see cref="CornerRadius" /> 依赖项属性的标识符。
        /// </returns>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ImageEx), new PropertyMetadata(default(CornerRadius), OnCornerRadiusChanged));

        /// <summary>
        /// 获取或设置一个值，该值表示该控件四个角的圆角程度。
        /// </summary>
        /// <returns>
        /// 该控件四个角的圆角程度。
        /// </returns>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        private static SKPath BuildCornerRadiusPath(float x, float y, float width, float height, float topLeft, float topRight, float bottomRight, float bottomLeft)
        {
            var path = new SKPath();
            
            var polyPoints = new List<SKPoint>();

            if (topLeft > 0)
            {
                var topLeftX = Math.Min(topLeft, width * (topLeft / (topLeft + topRight)));
                var topLeftY = Math.Min(topLeft, height * (topLeft / (topLeft + bottomLeft)));
                path.AddArc(new SKRect(x, y, x + topLeftX * 2, y + topLeftY * 2), 180, 90);
                polyPoints.Add(new SKPoint(x, y + topLeftY));
                polyPoints.Add(new SKPoint(x + topLeftX, y));
            }
            else
            {
                polyPoints.Add(new SKPoint(x, y));
            }

            if (topRight > 0)
            {
                var topRightX = Math.Min(topRight, width * (topRight / (topRight + topLeft)));
                var topRightY = Math.Min(topRight, height * (topRight / (topRight + bottomRight)));
                path.AddArc(new SKRect(x + width - topRightX * 2, y, x + width, y + topRightY * 2), 270, 90);
                polyPoints.Add(new SKPoint(x + width - topRightX, y));
                polyPoints.Add(new SKPoint(x + width, y + topRightY));
            }
            else
            {
                polyPoints.Add(new SKPoint(x + width, y));
            }

            if (bottomRight > 0)
            {
                var bottomRightX = Math.Min(bottomRight, width * (bottomRight / (bottomRight + bottomLeft)));
                var bottomRightY = Math.Min(bottomRight, height * (bottomRight / (bottomRight + topRight)));
                path.AddArc(new SKRect(x + width - bottomRightX * 2, y + height - bottomRightY * 2, x + width, y + height), 0, 90);
                polyPoints.Add(new SKPoint(x + width, y + height - bottomRightY));
                polyPoints.Add(new SKPoint(x + width - bottomRightX, y + height));
            }
            else
            {
                polyPoints.Add(new SKPoint(x + width, y + height));
            }

            if (bottomLeft > 0)
            {
                var bottomLeftX = Math.Min(bottomLeft, width * (bottomLeft / (bottomLeft + bottomRight)));
                var bottomLeftY = Math.Min(bottomLeft, height * (bottomLeft / (bottomLeft + topLeft)));
                path.AddArc(new SKRect(x, y + height - bottomLeftY * 2, x + bottomLeftX * 2, y + height), 90, 90);
                polyPoints.Add(new SKPoint(x + bottomLeftX, y + height));
                polyPoints.Add(new SKPoint(x, y + height - bottomLeftY));
            }
            else
            {
                polyPoints.Add(new SKPoint(x, y + height));
            }

            path.AddPoly(polyPoints.ToArray());

            path.Close();

            return path;
        }

        private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (ImageEx)d;

            obj.InvalidateCanvas();
        }
    }
}
